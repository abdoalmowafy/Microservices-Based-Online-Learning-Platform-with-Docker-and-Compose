using auth.Models;

namespace auth.DTOs.Responses
{
    public class InfoResponse
    {
        public required Guid Id { get; set; }
        public required string Username { get; set; }
        public required UserRole Role { get; set; }
        public required DateTime CreatedAt { get; set; }
    }
}
