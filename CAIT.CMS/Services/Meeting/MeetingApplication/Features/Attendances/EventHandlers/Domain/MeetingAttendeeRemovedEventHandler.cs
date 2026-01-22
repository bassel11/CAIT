using BuildingBlocks.Contracts.Meeting.Attendances.IntegrationEvents;
using MassTransit;
using MediatR;
using MeetingCore.Events.AttendanceEvents;
using Microsoft.Extensions.Logging;

namespace MeetingApplication.Features.Attendances.EventHandlers.Domain
{
    public class MeetingAttendeeRemovedEventHandler : INotificationHandler<MeetingAttendeeRemovedEvent>
    {
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly ILogger<MeetingAttendeeRemovedEventHandler> _logger;

        public MeetingAttendeeRemovedEventHandler(IPublishEndpoint publishEndpoint, ILogger<MeetingAttendeeRemovedEventHandler> logger)
        {
            _publishEndpoint = publishEndpoint;
            _logger = logger;
        }

        public async Task Handle(MeetingAttendeeRemovedEvent domainEvent, CancellationToken cancellationToken)
        {
            _logger.LogInformation("📢 Domain Event Handled: {DomainEvent}", nameof(MeetingAttendeeRemovedEvent));

            var integrationEvent = new MeetingAttendeeRemovedIntegrationEvent
            {
                MeetingId = domainEvent.MeetingId,
                MemberId = domainEvent.MemberId
            };

            await _publishEndpoint.Publish(integrationEvent, cancellationToken);
            _logger.LogInformation("🚀 Integration Event Published: {IntegrationEvent}", nameof(MeetingAttendeeRemovedIntegrationEvent));
        }
    }
}
