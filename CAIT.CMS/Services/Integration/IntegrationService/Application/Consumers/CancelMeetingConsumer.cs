using BuildingBlocks.Contracts.Meeting.Meetings.IntegrationEvents;
using IntegrationService.Application.Interfaces;
using MassTransit;

namespace IntegrationService.Application.Consumers
{
    public class CancelMeetingConsumer : IConsumer<MeetingCancelledIntegrationEvent>
    {
        private readonly IMeetingPlatformService _platformService;
        private readonly ILogger<CancelMeetingConsumer> _logger;

        public CancelMeetingConsumer(
            IMeetingPlatformService platformService,
            ILogger<CancelMeetingConsumer> logger)
        {
            _platformService = platformService;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<MeetingCancelledIntegrationEvent> context)
        {
            var msg = context.Message;
            _logger.LogInformation("⚠️ Integration: Cancelling Meeting {MeetingId}", msg.MeetingId);

            // إذا لم يكن الاجتماع مربوطاً بـ Outlook أصلاً، لا داعي لعمل شيء
            if (string.IsNullOrEmpty(msg.OutlookEventId))
            {
                _logger.LogWarning("Meeting {MeetingId} has no Outlook Event ID. Skipping integration.", msg.MeetingId);
                return;
            }

            await _platformService.CancelMeetingAsync(msg.OutlookEventId, msg.Reason);

            _logger.LogInformation("✅ Meeting {MeetingId} cancelled on Outlook/Teams.", msg.MeetingId);
        }
    }
}
