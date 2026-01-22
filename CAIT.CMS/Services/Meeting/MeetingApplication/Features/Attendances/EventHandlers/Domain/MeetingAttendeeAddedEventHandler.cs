using BuildingBlocks.Contracts.Meeting.Attendances.IntegrationEvents;
using MassTransit;
using MediatR;
using MeetingCore.Events.AttendanceEvents;
using Microsoft.Extensions.Logging;

namespace MeetingApplication.Features.Attendances.EventHandlers.Domain
{
    public class MeetingAttendeeAddedEventHandler : INotificationHandler<MeetingAttendeeAddedEvent>
    {
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly ILogger<MeetingAttendeeAddedEventHandler> _logger;

        public MeetingAttendeeAddedEventHandler(IPublishEndpoint publishEndpoint, ILogger<MeetingAttendeeAddedEventHandler> logger)
        {
            _publishEndpoint = publishEndpoint;
            _logger = logger;
        }

        public async Task Handle(MeetingAttendeeAddedEvent domainEvent, CancellationToken cancellationToken)
        {
            _logger.LogInformation("📢 Domain Event Handled: {DomainEvent}", nameof(MeetingAttendeeAddedEvent));

            var integrationEvent = new MeetingAttendeeAddedIntegrationEvent
            {
                MeetingId = domainEvent.MeetingId,
                MemberId = domainEvent.MemberId,
                Role = domainEvent.Role.ToString()
            };

            await _publishEndpoint.Publish(integrationEvent, cancellationToken);
            _logger.LogInformation("🚀 Integration Event Published: {IntegrationEvent}", nameof(MeetingAttendeeAddedIntegrationEvent));
        }
    }
}
