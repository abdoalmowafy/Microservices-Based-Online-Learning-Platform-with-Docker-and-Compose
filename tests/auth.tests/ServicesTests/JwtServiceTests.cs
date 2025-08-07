using auth.Data;
using auth.DTOs.Responses;
using auth.Models;
using auth.Services;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;

namespace auth.tests.ServicesTests
{
    public class JwtServiceTests
    {
        //Dependencies
        private readonly AuthDbContext _context;
        private readonly IConfiguration _configuration;
        //SUT
        private readonly JwtService _jwtService;
        public JwtServiceTests()
        {
            //Dependencies
            _context = new AuthDbContext(TestHelpers.CreateTestDbOptions());
            _configuration = TestHelpers.GetTestConfiguration();

            //SUT
            _jwtService = new JwtService(_context, _configuration);
        }


        [Theory]
        [InlineData(UserRole.User)]
        [InlineData(UserRole.Admin)]
        [InlineData(UserRole.SuperAdmin)]
        public void JwtService_GenerateAccessToken_ReturnsString(UserRole userRole)
        {
            // Arrange
            var userId = Guid.CreateVersion7();
            var handler = new JwtSecurityTokenHandler();
            var config = TestHelpers.GetTestConfiguration();

            // Act
            var token = _jwtService.GenerateAccessToken(userId, userRole);
            var jwt = handler.ReadJwtToken(token);

            // Assert
            token.Should().NotBeNullOrWhiteSpace();
            jwt.Claims.Should().ContainSingle(c => c.Type == JwtRegisteredClaimNames.Sub && c.Value == userId.ToString());
            jwt.Claims.Should().ContainSingle(c => c.Type == "role" && c.Value == userRole.ToString());
            jwt.Claims.Should().ContainSingle(c => c.Type == JwtRegisteredClaimNames.Iss && c.Value == config["JwtConfig:Issuer"]);
            jwt.Claims.Should().ContainSingle(c => c.Type == JwtRegisteredClaimNames.Aud && c.Value == config["JwtConfig:Audience"]);
            jwt.ValidTo.Should().BeAfter(DateTime.UtcNow);
            jwt.ValidTo.Should().Be(jwt.ValidFrom.AddMinutes(int.Parse(config["JwtConfig:AccessTokenValidityMins"]!)));
        }

        [Fact]
        public async Task JwtService_GenerateRefreshToken_ReturnsString()
        {
            // Arrange
            var userId = Guid.CreateVersion7();

            // Act
            var token = await _jwtService.GenerateRefreshTokenAsync(userId);

            // Assert
            token.Should().NotBeNullOrWhiteSpace();
        }

        [Fact]
        public async Task JwtService_GenerateTokensAsync_ReturnsObject()
        {
            // Arrange
            var userId = Guid.CreateVersion7();
            var role = UserRole.User;

            // Act
            var result = await _jwtService.GenerateTokensAsync(userId, role);

            // Assert
            result.Should().BeOfType<AuthResponse>();
            result.AccessToken.Should().NotBeNullOrWhiteSpace();
            result.RefreshToken.Should().NotBeNullOrWhiteSpace();
            result.ExpiresInMins.Should().BePositive();
        }

        [Fact]
        public async Task JwtService_RefreshAccessTokenAsync_ReturnsObject_WhenValidToken()
        {
            // Arrange
            var user = new AuthUser
            {
                Id = Guid.CreateVersion7(),
                Email = "testuser",
                PasswordHash = "hashedpassword",
                Role = UserRole.User
            };
            await _context.AuthUsers.AddAsync(user);
            await _context.SaveChangesAsync();
            var refreshToken = await _jwtService.GenerateRefreshTokenAsync(user.Id);

            // Act
            var result = await _jwtService.RefreshAccessTokenAsync(refreshToken);
            var authRefreshToken = await _context.AuthRefreshTokens
                .FirstOrDefaultAsync(rt => rt.RefreshToken == refreshToken);

            // Assert
            result.Should().BeOfType<AuthResponse>();
            result.AccessToken.Should().NotBeNullOrWhiteSpace();
            result.RefreshToken.Should().NotBeNullOrWhiteSpace();
            result.ExpiresInMins.Should().BePositive();

            authRefreshToken.Should().NotBeNull();
            authRefreshToken.IsRevoked.Should().BeTrue();
        }

        [Fact]
        public async Task JwtService_RefreshAccessTokenAsync_ThrowsException_WhenInvalidToken()
        {
            // Arrange

            // Act
            var act = async () => await _jwtService.RefreshAccessTokenAsync("invalid-token");

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("*Invalid or expired refresh token*");
        }
    }
}
