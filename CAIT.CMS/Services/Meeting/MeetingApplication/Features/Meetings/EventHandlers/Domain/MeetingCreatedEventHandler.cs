using BuildingBlocks.Contracts.Meeting.Meeting.IntegrationEvents;
using MassTransit;
using MediatR;
using MeetingCore.Events.MeetingEvents;
using Microsoft.Extensions.Logging;

namespace MeetingApplication.Features.Meetings.EventHandlers.Domain
{
    public class MeetingCreatedEventHandler : INotificationHandler<MeetingCreatedEvent>
    {
        private readonly IPublishEndpoint _publishEndpoint;
        //private readonly IFeatureManager _featureManager;
        private readonly ILogger<MeetingCreatedEventHandler> _logger;

        public MeetingCreatedEventHandler(
            IPublishEndpoint publishEndpoint,
            //IFeatureManager featureManager,
            ILogger<MeetingCreatedEventHandler> logger)
        {
            _publishEndpoint = publishEndpoint;
            //_featureManager = featureManager;
            _logger = logger;
        }

        public async Task Handle(MeetingCreatedEvent domainEvent, CancellationToken cancellationToken)
        {
            _logger.LogInformation("📢 Domain Event Handled: {DomainEvent} for Meeting {MeetingId}",
                nameof(MeetingCreatedEvent),
                domainEvent.MeetingId);

            // 1. التحقق من Feature Flag (اختياري حسب طلبك)
            // هذا مفيد إذا كنت تريد تفعيل نشر الأحداث تدريجياً
            //if (await _featureManager.IsEnabledAsync("MeetingIntegrationEvents"))
            //{
            // 2. التحويل (Mapping) من Domain Event إلى Integration Event
            // لاحظ: نقوم بالتحويل هنا يدوياً لأن Domain Event بياناته بسيطة الآن
            var integrationEvent = new MeetingCreatedIntegrationEvent
            {
                MeetingId = domainEvent.MeetingId,
                CommitteeId = domainEvent.CommitteeId,
                Title = domainEvent.Title,
                StartDate = domainEvent.StartDate,
                EndDate = domainEvent.EndDate,
                TimeZone = domainEvent.TimeZone,
                CreatedAt = domainEvent.CreatedAt ?? DateTime.UtcNow,

                // تحويل آمن للـ CreatedBy من string? إلى Guid
                CreatedBy = Guid.TryParse(domainEvent.CreatedBy, out var userId) ? userId : Guid.Empty
            };

            // 3. النشر عبر MassTransit
            // سيتم وضع الرسالة في Outbox Table أولاً بفضل إعدادات DbContext التي قمنا بها
            await _publishEndpoint.Publish(integrationEvent, cancellationToken);

            _logger.LogInformation("🚀 Integration Event Published: {IntegrationEvent}", nameof(MeetingCreatedIntegrationEvent));
            //}
            //    else
            //    {
            //        _logger.LogWarning("⚠️ Publishing MeetingCreatedIntegrationEvent is disabled via Feature Flags.");
            //    }
        }
    }
}
