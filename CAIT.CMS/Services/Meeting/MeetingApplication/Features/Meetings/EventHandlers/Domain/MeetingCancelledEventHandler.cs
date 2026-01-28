using BuildingBlocks.Contracts.Meeting.Meetings.IntegrationEvents;
using MassTransit;
using MediatR;
using MeetingApplication.Interfaces.Scheduling;
using MeetingCore.Events.MeetingEvents;
using Microsoft.Extensions.Logging;

namespace MeetingApplication.Features.Meetings.EventHandlers.Domain
{
    public class MeetingCancelledEventHandler : INotificationHandler<MeetingCancelledEvent>
    {
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly ILogger<MeetingCancelledEventHandler> _logger;
        private readonly IMeetingSchedulerGateway _schedulerGateway;

        public MeetingCancelledEventHandler(
            IPublishEndpoint publishEndpoint,
            ILogger<MeetingCancelledEventHandler> logger,
            IMeetingSchedulerGateway schedulerGateway)
        {
            _publishEndpoint = publishEndpoint;
            _logger = logger;
            _schedulerGateway = schedulerGateway;
        }

        public async Task Handle(MeetingCancelledEvent domainEvent, CancellationToken cancellationToken)
        {

            _logger.LogInformation("📢 Domain Event Handled: {DomainEvent}", nameof(MeetingCancelledEvent));


            _logger.LogInformation("🚫 Meeting {Id} Cancelled. Removing scheduled jobs...", domainEvent.MeetingId);

            await _schedulerGateway.CancelMeetingRemindersAsync(domainEvent.MeetingId.Value, cancellationToken);

            _logger.LogInformation("✅ Scheduled jobs removed for Meeting {Id}", domainEvent.MeetingId);


            var integrationEvent = new MeetingCancelledIntegrationEvent
            {
                MeetingId = domainEvent.MeetingId.Value,
                Reason = domainEvent.Reason,
                OutlookEventId = domainEvent.OutlookEventId,
                AttendeeIds = domainEvent.AttendeeIds.ToList()
            };

            await _publishEndpoint.Publish(integrationEvent, cancellationToken);
            _logger.LogInformation("🚀 Integration Event Published: {IntegrationEvent}", nameof(MeetingCancelledIntegrationEvent));
        }
    }
}
