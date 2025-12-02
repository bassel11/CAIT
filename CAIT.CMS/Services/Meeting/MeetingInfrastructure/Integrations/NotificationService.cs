using MailKit.Net.Smtp;
using MailKit.Security;
using MeetingApplication.Integrations;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;


namespace MeetingInfrastructure.Integrations
{
    public class SmtpSettings
    {
        public string Host { get; set; } = string.Empty;
        public int Port { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string FromEmail { get; set; } = string.Empty;
        public bool EnableSsl { get; set; }
    }

    public class NotificationService : INotificationService
    {
        private readonly SmtpSettings _settings;
        private readonly ILogger<NotificationService> _logger;

        public NotificationService(IOptions<SmtpSettings> options, ILogger<NotificationService> logger)
        {
            _settings = options.Value;
            _logger = logger;
        }

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("Meeting App", _settings.FromEmail));
                message.To.Add(new MailboxAddress("", to));
                message.Subject = subject;

                message.Body = new TextPart("html")
                {
                    Text = body
                };

                using var client = new SmtpClient();
                //await client.ConnectAsync(_settings.Host, _settings.Port, _settings.EnableSsl);
                await client.ConnectAsync(_settings.Host, _settings.Port, SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(_settings.Username, _settings.Password);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);

                _logger.LogInformation("[Email SENT] To: {To} | Subject: {Subject}", to, subject);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[Email ERROR]");
                throw;
            }
        }

        public Task SendPushAsync(Guid userId, string title, string body)
        {
            _logger.LogInformation("[Push] User {UserId} | {Title}", userId, title);
            return Task.CompletedTask;
        }
    }
}