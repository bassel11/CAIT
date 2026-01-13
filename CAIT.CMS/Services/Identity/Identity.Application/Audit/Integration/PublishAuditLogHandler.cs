using BuildingBlocks.Contracts.Audit;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Identity.Application.Audit.Integration
{
    public class PublishAuditLogHandler : INotificationHandler<AuditLogEvent>
    {
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly ILogger<PublishAuditLogHandler> _logger;

        public PublishAuditLogHandler(
            IPublishEndpoint publishEndpoint,
            ILogger<PublishAuditLogHandler> logger)
        {
            _publishEndpoint = publishEndpoint;
            _logger = logger;
        }

        public async Task Handle(AuditLogEvent notification, CancellationToken cancellationToken)
        {
            // تحويل الحدث المحلي إلى حدث تكاملي (Integration Event)
            var integrationEvent = new AuditLogCreatedIntegrationEvent(
                EventId: Guid.NewGuid(),
                ServiceName: "IdentityService",
                EntityName: notification.EntityName,
                PrimaryKey: notification.PrimaryKey,
                ActionType: notification.ActionType,

                // 👇 الحقول الجديدة
                CommitteeId: notification.CommitteeId,
                UserId: notification.UserId,
                UserName: notification.UserName,
                Email: notification.Email,
                Justification: notification.Justification,
                Severity: notification.Severity,

                OldValues: notification.OldValues,
                NewValues: notification.NewValues,
                ChangedColumns: notification.ChangedColumns,
                Timestamp: DateTime.UtcNow
            );

            // النشر للـ Outbox
            await _publishEndpoint.Publish(integrationEvent, cancellationToken);

            _logger.LogDebug("Audit Log queued for {Entity} (Committee: {CommitteeId})", notification.EntityName, notification.CommitteeId ?? "Global");
        }
    }

}
