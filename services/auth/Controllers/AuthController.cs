using auth.Data;
using auth.DTOs.Requests;
using auth.DTOs.Responses;
using auth.Events;
using auth.Events.Outgoing;
using auth.Helpers;
using auth.Models;
using auth.Services;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;

namespace auth.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AuthDbContext _context;
        private readonly IJwtService _jwtService;
        private readonly IEmailService _emailService;
        private readonly IPublishEndpoint _publishEndpoint;
        private const int EmailVerificationTokenLength = 64;
        private const int RefreshTokenLength = 64;
        private const int CanRequestNewTokenAfterMinutes = 5;
        private const int EmailVerificationTokenValidityDays = 7;
        public AuthController(AuthDbContext context, IJwtService jwtService, IEmailService emailService, IPublishEndpoint publishEndpoint)
        {
            _context = context;
            _jwtService = jwtService;
            _emailService = emailService;
            _publishEndpoint = publishEndpoint;
        }

        private async Task SendVerificationEmailAsync(AuthUser user)
        {
            var emailVerficationToken = new AuthEmailVerficationToken
            {
                EmailVerficationToken = RandomBytesGeneratorHelper.GenerateRandomBytes(EmailVerificationTokenLength),
                UserId = user.Id,
                ExpiresAt = DateTime.UtcNow.AddDays(EmailVerificationTokenValidityDays),
            };
            await _context.AuthEmailVerficationTokens.AddAsync(emailVerficationToken);

            await _emailService.SendEmailVerificationAsync(
                user.Email,
                emailVerficationToken.EmailVerficationToken
            );
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterAsync([FromBody] RegisterRequest request)
        {
            var userExists = await _context.AuthUsers
                .AsNoTracking()
                .AnyAsync(u => u.Email == request.Email);
            if (userExists)
                return Conflict("Email already exists.");

            var user = new AuthUser
            {
                Id = Guid.CreateVersion7(),
                Email = request.Email.Trim().ToLowerInvariant(),
                PasswordHash = PasswordHasher.Hash(request.Password),
                Role = UserRole.User // Default role, can be changed later
            };

            await _context.AuthUsers.AddAsync(user);
            await SendVerificationEmailAsync(user);
            await _context.SaveChangesAsync();

            return Created();
        }

        [HttpPost("verify-email/request")]
        public async Task<IActionResult> RequestVerifyEmailAsync([FromBody, EmailAddress] string email)
        {
            var user = await _context.AuthUsers
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Email == email.Trim().ToLowerInvariant());

            if (user is null)
                return NotFound(new { error = "User not found." });

            if (user.EmailIsVerified)
                return BadRequest(new { error = "Email is already verified." });

            var lastEmailVerficationToken = await _context.AuthEmailVerficationTokens
                .AsNoTracking()
                .LastOrDefaultAsync(evt => evt.UserId == user.Id && !evt.IsUsed && DateTime.UtcNow < evt.ExpiresAt);

            if (lastEmailVerficationToken is not null)
            {
                var CanRequestNewTokenAfter = lastEmailVerficationToken.CreatedAt.AddMinutes(CanRequestNewTokenAfterMinutes);
                if (DateTime.UtcNow < CanRequestNewTokenAfter)
                    return BadRequest(new { error = $"You can request a new verification email after {CanRequestNewTokenAfter - DateTime.UtcNow}" });
            }

            await SendVerificationEmailAsync(user);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Verification mail sent to this email!" });
        }

        [HttpGet("verify-email/verify/{token}")]
        public async Task<IActionResult> VerifyEmailAsync([FromRoute] string token)
        {
            if (string.IsNullOrWhiteSpace(token) || token.Length < EmailVerificationTokenLength)
                return BadRequest(new { error = "Invalid token format." });

            var emailVerficationToken = await _context.AuthEmailVerficationTokens
                .FirstOrDefaultAsync(evt => evt.EmailVerficationToken == token);

            if (emailVerficationToken is null || emailVerficationToken.IsUsed || emailVerficationToken.ExpiresAt < DateTime.UtcNow)
                return BadRequest(new { error = "Invalid or expired verification token." });

            await _context.Entry(emailVerficationToken)
                .Reference(evt => evt.User)
                .LoadAsync();

            var user = emailVerficationToken.User ??
                throw new InvalidOperationException("User not found for the provided verification token.");

            if (user.EmailIsVerified)
                return BadRequest(new { error = "Email is already verified." });

            emailVerficationToken.IsUsed = true;
            user.EmailIsVerified = true;
            _context.AuthUsers.Update(user);
            await _context.SaveChangesAsync();

            await _publishEndpoint.Publish(new UserCreated(
                user.Id,
                user.Email,
                user.CreatedAt
            ));

            return Ok(new { message = "Email verified successfully." });
        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync([FromBody] LoginRequest request)
        {
            var user = await _context.AuthUsers
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Email == request.Email.Trim().ToLowerInvariant());

            if (user is null || !PasswordHasher.Verify(request.Password, user.PasswordHash))
                return Unauthorized(new { error = "Invalid email or password." });

            if (!user.EmailIsVerified)
                return Unauthorized(new { error = "Email is not verified." });

            var response = await _jwtService.GenerateTokensAsync(user.Id, user.Role);

            return Ok(response);
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshAsync([FromBody] string refreshToken)
        {
            if (string.IsNullOrWhiteSpace(refreshToken) || refreshToken.Length < RefreshTokenLength)
                return BadRequest(new { error = "Invalid refresh token" });

            try
            {
                var response = await _jwtService.RefreshAccessTokenAsync(refreshToken);
                return Ok(response);
            }
            catch (InvalidOperationException e)
            {
                return Unauthorized(new { error = e.Message });
            }
        }


        [HttpGet("info"), Authorize]
        public async Task<IActionResult> InfoAsync()
        {
            var userId = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
            if (!Guid.TryParse(userId, out var userGuid))
                return Unauthorized(new { error = "Invalid or missing token." });

            var user = await _context.AuthUsers
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == userGuid);
            if (user is null)
                return NotFound(new { error = "User not found." });

            return Ok(new InfoResponse
            {
                Email = user.Email,
                CreatedAt = user.CreatedAt
            });
        }
    }
}
