using BuildingBlocks.Contracts.Meeting.Meeting.IntegrationEvents;
using MassTransit;
using MediatR;
using MeetingCore.Events.MeetingEvents;
using Microsoft.Extensions.Logging;

namespace MeetingApplication.Features.Meetings.EventHandlers.Domain
{
    public class MeetingCancelledEventHandler : INotificationHandler<MeetingCancelledEvent>
    {
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly ILogger<MeetingCancelledEventHandler> _logger;

        public MeetingCancelledEventHandler(IPublishEndpoint publishEndpoint, ILogger<MeetingCancelledEventHandler> logger)
        {
            _publishEndpoint = publishEndpoint;
            _logger = logger;
        }

        public async Task Handle(MeetingCancelledEvent domainEvent, CancellationToken cancellationToken)
        {
            _logger.LogInformation("📢 Domain Event Handled: {DomainEvent}", nameof(MeetingCancelledEvent));

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
