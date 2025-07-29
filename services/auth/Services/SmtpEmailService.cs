using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace auth.Services
{
    public class SmtpEmailService : IEmailService
    {
        private readonly IConfigurationSection _smtpEmailConfig;
        private readonly ILogger _logger;
        private readonly string _baseUrl = "https://localhost:5001";

        public SmtpEmailService(IConfiguration config, ILogger logger)
        {
            _smtpEmailConfig = config.GetSection("Email:Smtp") ?? 
                throw new InvalidOperationException("SMTP configuration is not missing.");
            _logger = logger;
        }

        private (string HOST, string FROM, string PORT, string USERNAME, string PASSWORD) GetSmtpEmailConfig()
        {
            var HOST = _smtpEmailConfig["Host"];
            var FROM = _smtpEmailConfig["From"];
            var PORT = _smtpEmailConfig["Port"];
            var USERNAME = _smtpEmailConfig["Username"];
            var PASSWORD = _smtpEmailConfig["Password"];
            if (string.IsNullOrWhiteSpace(HOST) || string.IsNullOrWhiteSpace(FROM) ||
                string.IsNullOrWhiteSpace(PORT) || string.IsNullOrWhiteSpace(USERNAME) ||
                string.IsNullOrWhiteSpace(PASSWORD))
            {
                throw new InvalidOperationException("SMTP configuration is not properly set.");
            }

            return new ( HOST, FROM, PORT, USERNAME, PASSWORD );
        }

        private async Task SendEmailAsync(string to, string subject, string htmlContent)
        {
            var (HOST, FROM, PORT, USERNAME, PASSWORD) = GetSmtpEmailConfig();

            var message = new MimeMessage();
            message.From.Add(MailboxAddress.Parse(FROM));
            message.To.Add(MailboxAddress.Parse(to));
            message.Subject = subject;
            message.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = htmlContent };

            using var client = new SmtpClient();
            await client.ConnectAsync(HOST, int.Parse(PORT), SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(USERNAME, PASSWORD);
            try
            {
                await client.SendAsync(message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email.");
            }
            await client.DisconnectAsync(true);
        }

        public async Task SendEmailVerificationAsync(string to, string verificationToken)
        {
            var subject = "Email Verification";
            var verificationUrl = $"{_baseUrl}/verify-email/{verificationToken}";
            var htmlContent = $@"
                <h1>Email Verification</h1>
                <p>Please click the link below to verify your email:</p>
                <a href='{verificationUrl}'>Verify Email</a>";
            await SendEmailAsync(to, subject, htmlContent);
        }
    }
}
