using MassTransit;
using MeetingCore.Events;
using Microsoft.Extensions.Logging;

namespace MeetingInfrastructure.Messaging.Consumers
{
    public class NotificationMoMApprovedConsumer : IConsumer<NotificationMoMApprovedEvent>
    {
        private readonly ILogger<NotificationMoMApprovedConsumer> _logger;
        public NotificationMoMApprovedConsumer(ILogger<NotificationMoMApprovedConsumer> logger)
        {
            _logger = logger;
        }
        public Task Consume(ConsumeContext<NotificationMoMApprovedEvent> context)
        {
            _logger.LogInformation("NotificationMoMApprovedEvent received: MeetingId={MeetingId}, Body={Body}",
                context.Message.Body, context.Message.Body);


            return Task.CompletedTask;
        }
    }
}
