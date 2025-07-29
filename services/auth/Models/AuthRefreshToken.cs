using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace auth.Models
{
    public class AuthRefreshToken
    {
        [Key] public Guid Id { get; set; }
        [Required] public required string RefreshToken { get; set; }
        [Required] public required Guid UserId { get; set; }
        [ForeignKey(nameof(UserId))] public AuthUser? User { get; set; }
        [Required] public required DateTime ExpiresAt { get; set; }
        [Required] public bool IsRevoked { get; set; } = false;
    }
}
