namespace auth.DTOs.Responses
{
    public class AuthResponse
    {
        public required string AccessToken { get; set; }
        public required string RefreshToken { get; set; }
        public required int ExpiresInMins { get; set; }
    }
}
