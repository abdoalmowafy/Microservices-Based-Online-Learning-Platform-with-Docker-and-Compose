using auth.Controllers;
using auth.Data;
using auth.DTOs.Requests;
using auth.DTOs.Responses;
using auth.Helpers;
using auth.Models;
using auth.Services;
using FakeItEasy;
using FluentAssertions;
using FluentAssertions.Primitives;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace auth.tests.ControllersTests
{
    public class AuthControllerTests
    {
        // Dependencies
        private readonly AuthDbContext _context;
        private readonly IJwtService _jwtService;
        private readonly IEmailService _emailService;
        // SUT (System Under Test)
        private readonly AuthController _authController;

        public AuthControllerTests()
        {
            // Dependencies
            _context = new AuthDbContext(TestHelpers.CreateTestDbOptions());

            _jwtService = A.Fake<IJwtService>();
            A.CallTo(() => _jwtService.GenerateTokensAsync(A<Guid>._, A<UserRole>._))
                .Returns(TestHelpers.FakeAuthResponse);
            A.CallTo(() => _jwtService.RefreshAccessTokenAsync(A<string>.Ignored))
             .ReturnsLazily((string input) =>
                 input == "Valid-Token-Valid-Token-Valid-Token-Valid-Token-Valid-Token-Vali" 
                 || input == "Valid*Token*Valid*Token*Valid*Token*Valid*Token*Valid*Token*Vali" ?
                 Task.FromResult(TestHelpers.FakeAuthResponse) :
                 throw new InvalidOperationException("Invalid or expired refresh token.")
             );

            _emailService = A.Fake<IEmailService>();

            // SUT
            _authController = new AuthController(_context, _jwtService, _emailService);
        }

        [Fact]
        public async Task AuthController_RegisterAsync_ReturnsConflict_WhenUserExistAsync()
        {
            // Arrange
            var request = new RegisterRequest
            {
                Email = "testuser",
                Password = "Test@123"
            };

            // Act
            await _authController.RegisterAsync(request);
            var result = await _authController.RegisterAsync(request);

            // Assert
            result.Should().BeOfType<ConflictObjectResult>();
        }

        [Fact]
        public async Task AuthController_RegisterAsync_ReturnsCreatedAt_WhenValidRequestAsync()
        {
            // Arrange
            var request = new RegisterRequest
            {
                Email = "testuser",
                Password = "Test@123"
            };

            // Act
            var result = await _authController.RegisterAsync(request);

            // Assert
            result.Should().BeOfType<CreatedAtActionResult>();
            var user = await _context.AuthUsers.SingleOrDefaultAsync(u => u.Email == request.Email);
            user.Should().NotBeNull();
            user.EmailIsVerified.Should().BeFalse();
            _context.AuthEmailVerficationTokens.Should().ContainSingle(evt => evt.UserId == user.Id);
        }

        [Fact]
        public async Task AuthController_RequestVerifyEmailAsync_ReturnsNotFound_WhenUserNotExistAsync()
        {
            // Arrange
            var email = "test@test.test";

            // Act
            var result = await _authController.RequestVerifyEmailAsync(email);
            
            // Assert
            result.Should().BeOfType<NotFoundObjectResult>();
            var notFoundResult = result as NotFoundObjectResult;
            notFoundResult!.Value.Should().BeEquivalentTo(new { error = "User not found." });
        }

        [Fact]
        public async Task AuthController_RequestVerifyEmailAsync_ReturnsBadRequest_WhenEmailIsVerifiedAsync()
        {
            // Arrange
            var user = new AuthUser
            {
                Email = "testuser",
                PasswordHash = PasswordHasher.Hash("Test@123"),
                EmailIsVerified = true
            };
            await _context.AuthUsers.AddAsync(user);
            await _context.SaveChangesAsync();

            // Act
            var result = await _authController.RequestVerifyEmailAsync(user.Email);
           
            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
            var badRequestResult = result as BadRequestObjectResult;
            badRequestResult!.Value.Should().BeEquivalentTo(new { error = "Email is already verified." });

        }

        [Fact]
        public async Task AuthController_RequestVerifyEmailAsync_ReturnsBadRequest_WhenTokenRequestedTooSoonAsync()
        {
            // Arrange
            var user = new AuthUser
            {
                Email = "testuser@test.com",
                PasswordHash = PasswordHasher.Hash("Test@123"),
            };
            await _context.AuthUsers.AddAsync(user);
            await _context.SaveChangesAsync();

            // Act
            await _authController.RequestVerifyEmailAsync(user.Email);
            var result = await _authController.RequestVerifyEmailAsync(user.Email);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
            var badRequestResult = result as BadRequestObjectResult;
            badRequestResult!.Value.Should().NotBeEquivalentTo(new { error = "Email is already verified." });
        }

        [Fact]
        public async Task AuthController_RequestVerifyEmailAsync_ReturnsOk_WhenValidRequestAsync()
        {
            // Arrange
            var user = new AuthUser
            {
                Email = "testuser@test.com",
                PasswordHash = PasswordHasher.Hash("Test@123"),
            };
            await _context.AuthUsers.AddAsync(user);
            await _context.SaveChangesAsync();

            // Act
            var result = await _authController.RequestVerifyEmailAsync(user.Email);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var badRequestResult = result as OkObjectResult;
            badRequestResult!.Value.Should().BeEquivalentTo(new { message = "Verification mail sent to this email!" });
        }

        [Theory]
        [InlineData("  ")]
        [InlineData("Less than 64 chars")]
        public async Task AuthController_VerifyEmailAsync_ReturnsBadRequest_WhenInvalidTokenFormatAsync(string token)
        {
            // Arrange
            var user = new AuthUser
            {
                Email = "testuser",
                PasswordHash = PasswordHasher.Hash("Test@123"),
            };
            var authEmailVerificationToken = new AuthEmailVerficationToken
            {
                EmailVerficationToken = RandomBytesGeneratorHelper.GenerateRandomBytes(64),
                UserId = user.Id,
                ExpiresAt = DateTime.UtcNow.AddDays(7)
            };
            await _context.AuthUsers.AddAsync(user);
            await _context.AuthEmailVerficationTokens.AddAsync(authEmailVerificationToken);
            await _context.SaveChangesAsync();

            // Act
            var result = await _authController.VerifyEmailAsync(token);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
            var badRequestResult = result as BadRequestObjectResult;
            badRequestResult!.Value.Should().BeEquivalentTo(new { error = "Invalid token format." });
        }

        public static TheoryData<DateTime, bool> GetTestCases() => new()
        {
            { DateTime.UtcNow.AddDays(-1), true },
            { DateTime.UtcNow.AddDays(7), true },
            { DateTime.UtcNow.AddDays(-1), false }
        };

        [Theory]
        [MemberData(nameof(GetTestCases))]
        public async Task AuthController_VerifyEmailAsync_ReturnsBadRequest_WhenInvalidOrExpiredTokenAsync(DateTime expiresAt, bool isUsed)
        {
            // Arrange
            var user = new AuthUser
            {
                Id = Guid.NewGuid(),
                Email = "testuser@test.com",
                PasswordHash = PasswordHasher.Hash("Test@123"),
            };
            var authEmailVerificationToken = new AuthEmailVerficationToken
            {
                EmailVerficationToken = RandomBytesGeneratorHelper.GenerateRandomBytes(64),
                UserId = user.Id,
                ExpiresAt = expiresAt,
                IsUsed = isUsed
            };

            await _context.AuthUsers.AddAsync(user);
            await _context.AuthEmailVerficationTokens.AddAsync(authEmailVerificationToken);
            await _context.SaveChangesAsync();

            // Act
            var result = await _authController.VerifyEmailAsync(user.Email);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
            var badRequestResult = result as BadRequestObjectResult;
            badRequestResult!.Value.Should().BeEquivalentTo(new { error = "Invalid token format." });
        }

        [Fact]
        public async Task AuthController_VerifyEmailAsync_ThrowsException_WhenValidTokenButMissingUserAsync()
        {
            // Arrange
            var authEmailVerificationToken = new AuthEmailVerficationToken
            {
                EmailVerficationToken = RandomBytesGeneratorHelper.GenerateRandomBytes(64),
                UserId = Guid.NewGuid(),
                ExpiresAt = DateTime.UtcNow.AddDays(7)
            };
            await _context.AuthEmailVerficationTokens.AddAsync(authEmailVerificationToken);
            await _context.SaveChangesAsync();

            // Act
            var act = () => _authController.VerifyEmailAsync(authEmailVerificationToken.EmailVerficationToken);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("User not found for the provided verification token.");
        }

        [Fact]
        public async Task AuthController_VerifyEmailAsync_ReturnBadRequest_WhenEmailAlreadyVerifiedAsync()
        {
            // Arrange
            var user = new AuthUser
            {
                Id = Guid.NewGuid(),
                Email = "testuser",
                PasswordHash = PasswordHasher.Hash("Test@123"),
                EmailIsVerified = true
            };

            var authEmailVerificationToken = new AuthEmailVerficationToken
            {
                EmailVerficationToken = RandomBytesGeneratorHelper.GenerateRandomBytes(64),
                UserId = user.Id,
                ExpiresAt = DateTime.UtcNow.AddDays(7)
            };
            await _context.AuthUsers.AddAsync(user);
            await _context.AuthEmailVerficationTokens.AddAsync(authEmailVerificationToken);
            await _context.SaveChangesAsync();

            // Act
            var result = await _authController.VerifyEmailAsync(authEmailVerificationToken.EmailVerficationToken);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
            var badRequestResult = result as BadRequestObjectResult;
            badRequestResult!.Value.Should().BeEquivalentTo(new { error = "Email is already verified." });
        }

        [Fact]
        public async Task AuthController_VerifyEmailAsync_ReturnsOk_WhenValidRequestAsync()
        {
            // Arrange
            var user = new AuthUser
            {
                Id = Guid.NewGuid(),
                Email = "testuser",
                PasswordHash = PasswordHasher.Hash("Test@123"),
            };

            var authEmailVerificationToken = new AuthEmailVerficationToken
            {
                EmailVerficationToken = RandomBytesGeneratorHelper.GenerateRandomBytes(64),
                UserId = user.Id,
                ExpiresAt = DateTime.UtcNow.AddDays(7)
            };
            await _context.AuthUsers.AddAsync(user);
            await _context.AuthEmailVerficationTokens.AddAsync(authEmailVerificationToken);
            await _context.SaveChangesAsync();

            // Act
            var result = await _authController.VerifyEmailAsync(authEmailVerificationToken.EmailVerficationToken);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var badRequestResult = result as OkObjectResult;
            badRequestResult!.Value.Should().BeEquivalentTo(new { message = "Email verified successfully." });
        }

        [Fact]
        public async Task AuthController_LoginAsync_ReturnsUnauthorized_WhenUserNotExistAsync()
        {
            // Arrange
            var request = new LoginRequest
            {
                Email = "testuser",
                Password = "Test@123"
            };

            // Act
            var result = await _authController.LoginAsync(request);

            // Assert
            result.Should().BeOfType<UnauthorizedObjectResult>();
        }

        [Fact]
        public async Task AuthController_LoginAsync_ReturnsUnauthorized_WhenWrongPasswordAsync()
        {
            // Arrange
            var user = new AuthUser
            {
                Email = "testuser",
                PasswordHash = PasswordHasher.Hash("Test@123")
            };

            var request = new LoginRequest
            {
                Email = user.Email,
                Password = "NotTest@123"
            };

            // Act
            await _context.AuthUsers.AddAsync(user);
            await _context.SaveChangesAsync();
            var result = await _authController.LoginAsync(request);

            // Assert
            result.Should().BeOfType<UnauthorizedObjectResult>();
        }

        [Fact]
        public async Task AuthController_LoginAsync_ReturnsOk_WhenValidAsync()
        {
            // Arrange
            var user = new AuthUser
            {
                Email = "testuser",
                PasswordHash = PasswordHasher.Hash("Test@123"),
                EmailIsVerified = true
            };

            var request = new LoginRequest
            {
                Email = user.Email,
                Password = "Test@123"
            };

            // Act
            await _context.AuthUsers.AddAsync(user);
            await _context.SaveChangesAsync();
            var result = await _authController.LoginAsync(request);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            okResult!.Value.Should().BeOfType<AuthResponse>();
        }

        [Fact]
        public async Task AuthController_RefreshAsync_ReturnsBadRequest_WhenMissingTokenAsync()
        {
            // Arrange

            // Act
            var result = await _authController.RefreshAsync("  ");

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task AuthController_RefreshAsync_ReturnsUnauthorized_WhenInvalidTokenAsync()
        {
            // Arrange

            // Act
            var result = await _authController.RefreshAsync("Invalid-Token-Invalid-Token-Invalid-Token-Invalid-Token-Invalid-");

            // Assert
            result.Should().BeOfType<UnauthorizedObjectResult>();
        }

        [Theory]
        [InlineData("Valid-Token-Valid-Token-Valid-Token-Valid-Token-Valid-Token-Vali")]
        [InlineData("Valid*Token*Valid*Token*Valid*Token*Valid*Token*Valid*Token*Vali")]
        public async Task AuthController_RefreshAsync_ReturnsOk_WhenValidTokenAsync(string refreshToken)
        {
            // Arrange

            // Act
            var result = await _authController.RefreshAsync(refreshToken);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            okResult!.Value.Should().BeOfType<AuthResponse>();
            var authResponse = okResult!.Value as AuthResponse;
            authResponse!.Should().BeEquivalentTo(TestHelpers.FakeAuthResponse);
        }
    }
}
