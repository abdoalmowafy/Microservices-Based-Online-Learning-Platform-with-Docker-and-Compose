using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace auth.Models
{
    public class AuthEmailVerficationToken
    {
        [Key] public Guid Id { get; set; }
        [Required] public required Guid UserId { get; set; }
        [ForeignKey(nameof(UserId))] public AuthUser? User { get; set; }
        [Required] public required string EmailVerficationToken { get; set; }
        [Required] public bool IsUsed { get; set; } = false;
        [Required] public required DateTime ExpiresAt { get; set; }
        [Required] public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
