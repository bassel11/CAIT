using BuildingBlocks.Contracts.Meeting.Attendances.IntegrationEvents;
using MassTransit;
using MediatR;
using MeetingCore.Events.AttendanceEvents;
using Microsoft.Extensions.Logging;

namespace MeetingApplication.Features.Attendances.EventHandlers.Domain
{
    public class MeetingAttendeeCheckedInEventHandler : INotificationHandler<MeetingAttendeeCheckedInEvent>
    {
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly ILogger<MeetingAttendeeCheckedInEventHandler> _logger;

        public MeetingAttendeeCheckedInEventHandler(IPublishEndpoint publishEndpoint, ILogger<MeetingAttendeeCheckedInEventHandler> logger)
        {
            _publishEndpoint = publishEndpoint;
            _logger = logger;
        }

        public async Task Handle(MeetingAttendeeCheckedInEvent domainEvent, CancellationToken cancellationToken)
        {
            _logger.LogInformation("📢 Domain Event Handled: {DomainEvent}", nameof(MeetingAttendeeCheckedInEvent));

            var integrationEvent = new MeetingAttendeeCheckedInIntegrationEvent
            {
                MeetingId = domainEvent.MeetingId,
                MemberId = domainEvent.MemberId,
                AttendanceStatus = domainEvent.Status.ToString(),
                IsRemote = domainEvent.IsRemote,
                Timestamp = domainEvent.Timestamp
            };

            await _publishEndpoint.Publish(integrationEvent, cancellationToken);
            _logger.LogInformation("🚀 Integration Event Published: {IntegrationEvent}", nameof(MeetingAttendeeCheckedInIntegrationEvent));
        }
    }
}
