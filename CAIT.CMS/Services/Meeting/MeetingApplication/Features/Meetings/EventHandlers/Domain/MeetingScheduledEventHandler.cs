using BuildingBlocks.Contracts.Meeting.Meeting.IntegrationEvents;
using MassTransit;
using MediatR;
using MeetingCore.Events.MeetingEvents;
using Microsoft.Extensions.Logging;

namespace MeetingApplication.Features.Meetings.EventHandlers.Domain
{
    public class MeetingScheduledEventHandler : INotificationHandler<MeetingScheduledEvent>
    {
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly ILogger<MeetingScheduledEventHandler> _logger;

        public MeetingScheduledEventHandler(IPublishEndpoint publishEndpoint, ILogger<MeetingScheduledEventHandler> logger)
        {
            _publishEndpoint = publishEndpoint;
            _logger = logger;
        }

        public async Task Handle(MeetingScheduledEvent domainEvent, CancellationToken cancellationToken)
        {
            _logger.LogInformation("📢 Domain Event Handled: {DomainEvent}", nameof(MeetingScheduledEvent));

            var integrationEvent = new MeetingScheduledIntegrationEvent
            {
                MeetingId = domainEvent.MeetingId.Value,
                CommitteeId = domainEvent.CommitteeId.Value,
                Title = domainEvent.Title,
                StartDate = domainEvent.StartDate,
                EndDate = domainEvent.EndDate,
                // تحويل IReadOnlyList<Guid> إلى List<Guid> لـ MassTransit
                AttendeeIds = domainEvent.AttendeeIds.ToList()
            };

            await _publishEndpoint.Publish(integrationEvent, cancellationToken);
            _logger.LogInformation("🚀 Integration Event Published: {IntegrationEvent}", nameof(MeetingScheduledIntegrationEvent));
        }
    }
}
