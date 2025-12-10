using BuildingBlocks.Contracts.Notifications;
using MassTransit;
using NotificationService.Services;

namespace NotificationService.Consumers.SendEmail
{
    public class MoMApprovedNotificationConsumer : IConsumer<MoMApprovedNotification>
    {
        private readonly ILogger<MoMApprovedNotificationConsumer> _logger;
        private readonly IEmailService _emailService;

        public MoMApprovedNotificationConsumer(
            ILogger<MoMApprovedNotificationConsumer> logger,
            IEmailService emailService)
        {
            _logger = logger;
            _emailService = emailService;
        }

        public async Task Consume(ConsumeContext<MoMApprovedNotification> context)
        {
            var message = context.Message;
            _logger.LogInformation("Sending email to {Email}", message.To);

            await _emailService.SendEmailAsync(message.To, message.Subject, message.Body);
        }
    }
}
