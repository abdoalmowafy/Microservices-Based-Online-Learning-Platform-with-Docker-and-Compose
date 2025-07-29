using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace auth.Models
{
    public class AuthUser
    {
        [Key] public Guid Id { get; set; }
        [Required, EmailAddress] public required string Email { get; set; }
        [Required] public required string PasswordHash { get; set; }
        [Required] public bool EmailIsVerified { get; set; } = false;
        [Required] public UserRole Role { get; set; } = UserRole.Student;
        [Required] public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum UserRole
    {
        Student,
        Teacher,
        Admin
    }
}
