using BuildingBlocks.Contracts.Meeting.Attendances.IntegrationEvents;
using MassTransit;
using MediatR;
using MeetingCore.Events.AttendanceEvents;
using Microsoft.Extensions.Logging;

namespace MeetingApplication.Features.Attendances.EventHandlers.Domain
{
    public class MeetingAttendeeRSVPedEventHandler : INotificationHandler<MeetingAttendeeRSVPedEvent>
    {
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly ILogger<MeetingAttendeeRSVPedEventHandler> _logger;

        public MeetingAttendeeRSVPedEventHandler(IPublishEndpoint publishEndpoint, ILogger<MeetingAttendeeRSVPedEventHandler> logger)
        {
            _publishEndpoint = publishEndpoint;
            _logger = logger;
        }

        public async Task Handle(MeetingAttendeeRSVPedEvent domainEvent, CancellationToken cancellationToken)
        {
            _logger.LogInformation("📢 Domain Event Handled: {DomainEvent}", nameof(MeetingAttendeeRSVPedEvent));

            var integrationEvent = new MeetingAttendeeRSVPedIntegrationEvent
            {
                MeetingId = domainEvent.MeetingId,
                MemberId = domainEvent.MemberId,
                RSVPStatus = domainEvent.RSVP.ToString(), // التحويل من Enum إلى String
                Timestamp = domainEvent.Timestamp
            };

            await _publishEndpoint.Publish(integrationEvent, cancellationToken);
            _logger.LogInformation("🚀 Integration Event Published: {IntegrationEvent}", nameof(MeetingAttendeeRSVPedIntegrationEvent));
        }
    }
}
