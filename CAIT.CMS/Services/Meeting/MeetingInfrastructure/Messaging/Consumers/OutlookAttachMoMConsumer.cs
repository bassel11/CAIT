using BuildingBlocks.Contracts.Outlook;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace MeetingInfrastructure.Messaging.Consumers
{
    public class OutlookAttachMoMConsumer : IConsumer<AttachMoMToOutlookEvent>
    {
        private readonly ILogger<OutlookAttachMoMConsumer> _logger;

        public OutlookAttachMoMConsumer(ILogger<OutlookAttachMoMConsumer> logger)
        {
            _logger = logger;
        }

        public Task Consume(ConsumeContext<AttachMoMToOutlookEvent> context)
        {
            _logger.LogInformation("OutlookAttachMoMEvent received: MeetingId={MeetingId}, Url={Url}",
                context.Message.MeetingId, context.Message.PdfUrl);

            // TODO: استدعاء Outlook API لإرفاق MoM
            return Task.CompletedTask;
        }
    }
}
