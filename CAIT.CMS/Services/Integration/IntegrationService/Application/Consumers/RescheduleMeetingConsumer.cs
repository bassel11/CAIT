using BuildingBlocks.Contracts.Meeting.Meeting.IntegrationEvents;
using IntegrationService.Application.Interfaces;
using MassTransit;

namespace IntegrationService.Application.Consumers
{
    public class RescheduleMeetingConsumer : IConsumer<MeetingRescheduledIntegrationEvent>
    {
        private readonly IMeetingPlatformService _platformService;
        private readonly ILogger<RescheduleMeetingConsumer> _logger;

        public RescheduleMeetingConsumer(
            IMeetingPlatformService platformService,
            ILogger<RescheduleMeetingConsumer> logger)
        {
            _platformService = platformService;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<MeetingRescheduledIntegrationEvent> context)
        {
            var msg = context.Message;
            _logger.LogInformation("🔄 Integration: Rescheduling Meeting {MeetingId}", msg.MeetingId);

            if (string.IsNullOrEmpty(msg.OutlookEventId))
            {
                _logger.LogWarning("Meeting {MeetingId} not integrated. Skipping.", msg.MeetingId);
                return;
            }

            await _platformService.UpdateMeetingTimeAsync(
                msg.OutlookEventId,
                msg.NewStartDate,
                msg.NewEndDate);

            _logger.LogInformation("✅ Meeting {MeetingId} rescheduled on Outlook.", msg.MeetingId);
        }
    }
}
