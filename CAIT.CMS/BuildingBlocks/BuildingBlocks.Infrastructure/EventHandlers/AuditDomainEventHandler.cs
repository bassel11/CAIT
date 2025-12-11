using BuildingBlocks.Contracts.Audit;
using MassTransit;
using MediatR;
using System.Reflection;

namespace BuildingBlocks.Infrastructure.EventHandlers
{
    // Generic Handler يعمل لأي حدث Auditable في أي Microservice
    public class AuditDomainEventHandler<TEvent> : INotificationHandler<TEvent>
        where TEvent : class, IAuditableEvent
    {
        private readonly IPublishEndpoint _publishEndpoint;

        public AuditDomainEventHandler(IPublishEndpoint publishEndpoint)
        {
            _publishEndpoint = publishEndpoint;
        }

        public async Task Handle(TEvent notification, CancellationToken ct)
        {
            // جلب اسم الخدمة تلقائياً (MeetingService, IdentityService...)
            var serviceName = Assembly.GetEntryAssembly()?.GetName().Name ?? "UnknownService";

            // النشر هنا يذهب لجدول Outbox مباشرة لأننا داخل Transaction
            await _publishEndpoint.Publish<IAuditLogCreated>(new
            {
                EventId = notification.EventId,
                UserId = notification.UserId,
                ServiceName = serviceName,
                EntityName = notification.EntityName,
                ActionType = notification.ActionType,
                PrimaryKey = notification.PrimaryKey,
                Timestamp = notification.OccurredAt
            }, ct);
        }
    }
}
