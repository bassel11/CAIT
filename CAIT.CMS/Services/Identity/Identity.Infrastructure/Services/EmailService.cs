using Identity.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;

namespace Identity.Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendMfaCodeAsync(string email, string code)
        {
            string subject = "Your MFA Code";
            string body = $"Your one-time MFA code is: {code}. It is valid for 5 minutes.";

            await SendEmailAsync(email, subject, body);
        }

        public async Task SendEmailAsync(string email, string subject, string body)
        {
            var smtpSection = _config.GetSection("Smtp");
            string host = smtpSection["Host"] ?? "localhost";
            int port = int.TryParse(smtpSection["Port"], out int p) ? p : 25;
            string username = smtpSection["Username"] ?? "";
            string password = smtpSection["Password"] ?? "";
            bool enableSsl = bool.TryParse(smtpSection["EnableSsl"], out bool ssl) ? ssl : true;
            string fromEmail = smtpSection["FromEmail"] ?? "no-reply@cait.gov";

            using var client = new SmtpClient(host, port)
            {
                Credentials = new NetworkCredential(username, password),
                EnableSsl = enableSsl
            };

            var mailMessage = new MailMessage(fromEmail, email, subject, body);
            mailMessage.IsBodyHtml = false;

            await client.SendMailAsync(mailMessage);
        }
    }
}
