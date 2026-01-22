using BuildingBlocks.Contracts.Meeting.Meeting.IntegrationEvents;
using MassTransit;
using MediatR;
using MeetingCore.Events.MeetingEvents;
using Microsoft.Extensions.Logging;

namespace MeetingApplication.Features.Meetings.EventHandlers.Domain
{
    public class MeetingCompletedEventHandler : INotificationHandler<MeetingCompletedEvent>
    {
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly ILogger<MeetingCompletedEventHandler> _logger;

        public MeetingCompletedEventHandler(IPublishEndpoint publishEndpoint, ILogger<MeetingCompletedEventHandler> logger)
        {
            _publishEndpoint = publishEndpoint;
            _logger = logger;
        }

        public async Task Handle(MeetingCompletedEvent domainEvent, CancellationToken cancellationToken)
        {
            _logger.LogInformation("📢 Domain Event Handled: {DomainEvent}", nameof(MeetingCompletedEvent));

            var integrationEvent = new MeetingCompletedIntegrationEvent
            {
                MeetingId = domainEvent.MeetingId, // هنا هو Guid أصلاً حسب تعريفك
                CompletedAt = domainEvent.CompletedAt
            };

            await _publishEndpoint.Publish(integrationEvent, cancellationToken);
            _logger.LogInformation("🚀 Integration Event Published: {IntegrationEvent}", nameof(MeetingCompletedIntegrationEvent));
        }
    }
}
