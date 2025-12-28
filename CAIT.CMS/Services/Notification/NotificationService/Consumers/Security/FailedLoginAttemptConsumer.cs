using BuildingBlocks.Contracts.SecurityEvents;
using MassTransit;
using NotificationService.Services;

namespace NotificationService.Consumers.Security
{
    public class FailedLoginAttemptConsumer : IConsumer<FailedLoginAttemptEvent>
    {
        private readonly ILogger<FailedLoginAttemptConsumer> _logger;
        private readonly IEmailService _emailService;

        public FailedLoginAttemptConsumer(
            ILogger<FailedLoginAttemptConsumer> logger,
            IEmailService emailService)
        {
            _logger = logger;
            _emailService = emailService;
        }

        public async Task Consume(ConsumeContext<FailedLoginAttemptEvent> context)
        {
            var message = context.Message;

            if (!message.ThresholdExceeded)
                return;

            _logger.LogWarning(
                "Security alert: Failed login threshold exceeded for user {UserName}",
                message.UserName);

            var subject = "🚨 Security Alert: Multiple Failed Login Attempts";

            var body = $@"
                <h3>Security Alert</h3>
                <p><strong>User:</strong> {message.UserName}</p>
                <p><strong>Failed Attempts:</strong> {message.FailedCount}</p>
                <p><strong>IP Address:</strong> {message.IpAddress}</p>
                <p><strong>Time:</strong> {message.OccurredAt:u}</p>
                <p>Please investigate immediately.</p>
            ";

            // ⚠️ في المرحلة التالية يمكن جلب الإيميلات من DB أو Config
            await _emailService.SendEmailAsync(
                to: "bassel.as19@gmail.com", //superadmin@system.local
                subject: subject,
                body: body
            );
        }
    }
}
