using auth.Data;
using auth.DTOs.Responses;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace auth.tests
{
    public static class TestHelpers
    {
        public static DbContextOptions<AuthDbContext> CreateTestDbOptions() =>
            new DbContextOptionsBuilder<AuthDbContext>()
                .UseInMemoryDatabase($"TestDb_{Guid.NewGuid()}")
                .Options;

        public static IConfiguration GetTestConfiguration() =>
            new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    {"JwtConfig:Issuer", "TestIssuer"},
                    {"JwtConfig:Audience", "TestAudience"},
                    {"JwtConfig:Key", "TestKeysTestKeysTestKeysTestKeys"},
                    {"JwtConfig:AccessTokenValidityMins", "60"},
                    {"JwtConfig:RefreshTokenValidityDays", "7"}
                }).Build();

        public static AuthResponse FakeAuthResponse => new()
        {
            AccessToken = "Fake Access Token",
            RefreshToken = "Fake Refresh Token",
            ExpiresInMins = 60
        };
    }
}
