using MassTransit;
using MeetingCore.Events;
using Microsoft.Extensions.Logging;

namespace MeetingInfrastructure.Messaging.Consumers
{
    public class OutlookAttachMoMConsumer : IConsumer<OutlookAttachMoMEvent>
    {
        private readonly ILogger<OutlookAttachMoMConsumer> _logger;

        public OutlookAttachMoMConsumer(ILogger<OutlookAttachMoMConsumer> logger)
        {
            _logger = logger;
        }

        public Task Consume(ConsumeContext<OutlookAttachMoMEvent> context)
        {
            _logger.LogInformation("OutlookAttachMoMEvent received: MeetingId={MeetingId}, Url={Url}",
                context.Message.MeetingId, context.Message.Url);

            // TODO: استدعاء Outlook API لإرفاق MoM
            return Task.CompletedTask;
        }
    }
}
