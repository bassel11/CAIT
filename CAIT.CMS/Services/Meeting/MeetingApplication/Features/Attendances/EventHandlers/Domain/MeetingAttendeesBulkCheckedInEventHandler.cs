using BuildingBlocks.Contracts.Meeting.Attendances.IntegrationEvents;
using MassTransit;
using MediatR;
using MeetingCore.Events.AttendanceEvents;
using Microsoft.Extensions.Logging;

namespace MeetingApplication.Features.Attendances.EventHandlers.Domain
{
    public class MeetingAttendeesBulkCheckedInEventHandler : INotificationHandler<MeetingAttendeesBulkCheckedInEvent>
    {
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly ILogger<MeetingAttendeesBulkCheckedInEventHandler> _logger;

        public MeetingAttendeesBulkCheckedInEventHandler(IPublishEndpoint publishEndpoint, ILogger<MeetingAttendeesBulkCheckedInEventHandler> logger)
        {
            _publishEndpoint = publishEndpoint;
            _logger = logger;
        }

        public async Task Handle(MeetingAttendeesBulkCheckedInEvent domainEvent, CancellationToken cancellationToken)
        {
            _logger.LogInformation("📢 Domain Event Handled: {DomainEvent}", nameof(MeetingAttendeesBulkCheckedInEvent));

            // تحويل القائمة الداخلية إلى DTOs
            var integrationItems = domainEvent.CheckedInItems
                .Select(x => new BulkCheckInItemDto(x.MemberId, x.Status.ToString()))
                .ToList();

            var integrationEvent = new MeetingAttendeesBulkCheckedInIntegrationEvent
            {
                MeetingId = domainEvent.MeetingId,
                Items = integrationItems,
                Timestamp = domainEvent.Timestamp
            };

            await _publishEndpoint.Publish(integrationEvent, cancellationToken);
            _logger.LogInformation("🚀 Integration Event Published: {IntegrationEvent}", nameof(MeetingAttendeesBulkCheckedInIntegrationEvent));
        }
    }
}
