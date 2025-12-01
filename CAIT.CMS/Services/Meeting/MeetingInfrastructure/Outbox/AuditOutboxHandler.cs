using MeetingApplication.Integrations;
using MeetingCore.Entities;
using System.Text.Json;

namespace MeetingInfrastructure.Outbox
{
    public class AuditOutboxHandler : IOutboxHandler
    {
        private readonly IAuditService _audit;
        private readonly JsonSerializerOptions _jsonOptions = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

        public AuditOutboxHandler(IAuditService audit) { _audit = audit; }

        public async Task HandleAsync(OutboxMessage message, CancellationToken ct)
        {
            var payload = JsonSerializer.Deserialize<AuditLog>(message.Payload);
            await _audit.RecordAsync(payload);
        }
    }

}
