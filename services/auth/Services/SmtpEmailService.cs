using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace auth.Services
{
    public class SmtpEmailService : IEmailService
    {
        private readonly IConfigurationSection _smtpEmailConfig;
        private readonly ILogger _logger;
        private readonly string _baseUrl = "localhost:3000";

        public SmtpEmailService(IConfiguration config, ILogger<SmtpEmailService> logger)
        {
            _smtpEmailConfig = config.GetSection("Email:Smtp") ?? 
                throw new InvalidOperationException("SMTP configuration is not missing.");
            _logger = logger;
        }

        private (string HOST, int PORT, SecureSocketOptions SSO, string FROM, string? USERNAME, string? PASSWORD) GetSmtpEmailConfig()
        {
            var HOST = _smtpEmailConfig["Host"];
            var rawPORT = _smtpEmailConfig["Port"];
            var rawSSO = _smtpEmailConfig["SecureSocketOptions"] ?? "None";
            var FROM = _smtpEmailConfig["From"];
            var USERNAME = _smtpEmailConfig["Username"];
            var PASSWORD = _smtpEmailConfig["Password"];
            if (string.IsNullOrWhiteSpace(HOST) || !int.TryParse(rawPORT, out int PORT) 
                || Enum.TryParse<SecureSocketOptions>(rawSSO, out var SSO) || string.IsNullOrWhiteSpace(FROM))
            {
                throw new InvalidOperationException("SMTP configuration is not properly set.");
            }

            return new ( HOST, PORT, SSO, FROM, USERNAME, PASSWORD );
        }

        private async Task SendEmailAsync(string to, string subject, string htmlContent)
        {
            var (HOST, PORT, SSO, FROM, USERNAME, PASSWORD) = GetSmtpEmailConfig();

            var message = new MimeMessage();
            message.From.Add(MailboxAddress.Parse(FROM));
            message.To.Add(MailboxAddress.Parse(to));
            message.Subject = subject;
            message.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = htmlContent };

            using var client = new SmtpClient();
            await client.ConnectAsync(HOST, PORT, SSO);
            
            if(!string.IsNullOrWhiteSpace(USERNAME) && !string.IsNullOrWhiteSpace(PASSWORD))
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
            var verificationUrl = $"{_baseUrl}/api/auth/verify-email/verify/{verificationToken}";
            var htmlContent = $@"
                <h1>Email Verification</h1>
                <p>Please click the link below to verify your email:</p>
                <a href='{verificationUrl}'>Verify Email</a>";
            await SendEmailAsync(to, subject, htmlContent);
        }
    }
}
