using auth.Data;
using auth.Models;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.EntityFrameworkCore;
using auth.DTOs.Responses;
using auth.Helpers;

namespace auth.Services
{
    public class JwtService : IJwtService
    {
        private readonly AuthDbContext _context;
        private readonly IConfigurationSection _jwtConfig;
        public JwtService(AuthDbContext context, IConfiguration configuration)
        {
            _context = context;
            _jwtConfig = configuration.GetSection("JwtConfig");
        }


        public string GenerateAccessToken(Guid userId, UserRole userRole)
        {
            var issuer = _jwtConfig["Issuer"];
            var audience = _jwtConfig["Audience"];
            if (string.IsNullOrWhiteSpace(issuer) || string.IsNullOrWhiteSpace(audience))
                throw new InvalidOperationException("JWT issuer or audience is not configured.");

            var key = _jwtConfig["Key"];
            if (string.IsNullOrWhiteSpace(key) || key.Length < 32)
                throw new InvalidOperationException("JWT signing key is missing or too short.");

            var AccessTokenValidityMins = _jwtConfig["AccessTokenValidityMins"];
            if (!int.TryParse(AccessTokenValidityMins, out int atValdityMinsInt) || atValdityMinsInt <= 0)
                throw new InvalidOperationException("Access token validity must be a positive number.");

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = issuer,
                Audience = audience,
                Expires = DateTime.UtcNow.AddMinutes(atValdityMinsInt),
                Subject = new ClaimsIdentity([
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.CreateVersion7().ToString()),
                    new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
                    new Claim(ClaimTypes.Role, userRole.ToString())
                ]),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key)),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = tokenHandler.CreateToken(tokenDescriptor);
            var accessToken = tokenHandler.WriteToken(securityToken);
            
            return accessToken;
        }

        public async Task<string> GenerateRefreshTokenAsync(Guid userId)
        {
            var RefreshTokenValidityDays = _jwtConfig["RefreshTokenValidityDays"];
            if (!int.TryParse(RefreshTokenValidityDays, out int rtValdityDaysInt) || rtValdityDaysInt <= 0)
                throw new InvalidOperationException("Refresh token validity must be a positive number.");

            var refreshToken = RandomBytesGeneratorHelper.GenerateRandomBytes(64);

            var authRefreshToken = new AuthRefreshToken
            {
                UserId = userId,
                RefreshToken = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddDays(rtValdityDaysInt)
            };

            await _context.AuthRefreshTokens.AddAsync(authRefreshToken);
            await _context.SaveChangesAsync();

            return authRefreshToken.RefreshToken;
        }

        public async Task<AuthResponse> GenerateTokensAsync(Guid userId, UserRole userRole) => 
            new AuthResponse {
            AccessToken = GenerateAccessToken(userId, userRole),
            RefreshToken = await GenerateRefreshTokenAsync(userId),
            ExpiresInMins = int.Parse(_jwtConfig["AccessTokenValidityMins"]!)
            };
        

        public async Task<AuthResponse> RefreshAccessTokenAsync(string refreshToken)
        {
            var authRefreshToken = await _context.AuthRefreshTokens
                .FirstOrDefaultAsync(rt => rt.RefreshToken == refreshToken && !rt.IsRevoked && rt.ExpiresAt > DateTime.UtcNow)
                ?? throw new InvalidOperationException("Invalid or expired refresh token.");
            
            authRefreshToken.IsRevoked = true;

            await _context.Entry(authRefreshToken)
                .Reference(rt => rt.User)
                .LoadAsync();

            return await GenerateTokensAsync(authRefreshToken.UserId, authRefreshToken.User!.Role);
        }
    }
}
