using auth.DTOs.Responses;
using auth.Models;

namespace auth.Services
{
    public interface IJwtService
    {
        string GenerateAccessToken(Guid userId, UserRole userRole);
        Task<string> GenerateRefreshTokenAsync(Guid userId);
        Task<AuthResponse> GenerateTokensAsync(Guid userId, UserRole userRole);
        Task<AuthResponse> RefreshAccessTokenAsync(string refreshToken);
    }
}
