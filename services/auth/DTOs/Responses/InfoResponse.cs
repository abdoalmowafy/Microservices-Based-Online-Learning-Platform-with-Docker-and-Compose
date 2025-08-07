using auth.Models;

namespace auth.DTOs.Responses
{
    public class InfoResponse
    {
        public required string Email { get; set; }
        public required DateTime CreatedAt { get; set; }
    }
}
