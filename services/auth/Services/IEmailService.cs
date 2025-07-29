namespace auth.Services
{
    public interface IEmailService
    {
        public Task SendEmailVerificationAsync(string to, string verficationToken);
    }
}
