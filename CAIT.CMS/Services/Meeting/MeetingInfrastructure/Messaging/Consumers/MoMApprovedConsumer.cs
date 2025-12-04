using MassTransit;
using MeetingCore.Events;
using Microsoft.Extensions.Logging;

namespace MeetingInfrastructure.Messaging.Consumers
{
    public class MoMApprovedConsumer : IConsumer<MoMApprovedEvent>
    {
        private readonly ILogger<MoMApprovedConsumer> _logger;

        public MoMApprovedConsumer(ILogger<MoMApprovedConsumer> logger)
        {
            _logger = logger;
        }

        public Task Consume(ConsumeContext<MoMApprovedEvent> context)
        {
            _logger.LogInformation("MoMApprovedEvent received: MoMId={MoMId}, MeetingId={MeetingId}",
                context.Message.MoMId, context.Message.MeetingId);

            // TODO: إرسال Integration / Notification لاحقًا
            return Task.CompletedTask;
        }
    }
}
