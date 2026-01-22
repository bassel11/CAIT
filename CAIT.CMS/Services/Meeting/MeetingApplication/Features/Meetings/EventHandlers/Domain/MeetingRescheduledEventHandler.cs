using BuildingBlocks.Contracts.Meeting.Meeting.IntegrationEvents;
using MassTransit;
using MediatR;
using MeetingCore.Events.MeetingEvents;
using Microsoft.Extensions.Logging;

namespace MeetingApplication.Features.Meetings.EventHandlers.Domain
{
    public class MeetingRescheduledEventHandler : INotificationHandler<MeetingRescheduledEvent>
    {
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly ILogger<MeetingRescheduledEventHandler> _logger;

        public MeetingRescheduledEventHandler(IPublishEndpoint publishEndpoint, ILogger<MeetingRescheduledEventHandler> logger)
        {
            _publishEndpoint = publishEndpoint;
            _logger = logger;
        }

        public async Task Handle(MeetingRescheduledEvent domainEvent, CancellationToken cancellationToken)
        {
            _logger.LogInformation("📢 Domain Event Handled: {DomainEvent}", nameof(MeetingRescheduledEvent));

            var integrationEvent = new MeetingRescheduledIntegrationEvent
            {
                MeetingId = domainEvent.MeetingId.Value,
                NewStartDate = domainEvent.NewStartDate,
                NewEndDate = domainEvent.NewEndDate,
                OutlookEventId = domainEvent.OutlookEventId
            };

            await _publishEndpoint.Publish(integrationEvent, cancellationToken);
            _logger.LogInformation("🚀 Integration Event Published: {IntegrationEvent}", nameof(MeetingRescheduledIntegrationEvent));
        }
    }
}
