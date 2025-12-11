using Audit.Application.Contracts;
using Audit.Domain.Entities;
using BuildingBlocks.Contracts.Audit;
using MassTransit;
using System.Text.Json;

namespace Audit.Infrastructure.Consumers
{
    public class AuditEventConsumer : IConsumer<IAuditLogCreated>
    {
        private readonly IAuditStore _store;

        public AuditEventConsumer(IAuditStore store) => _store = store;

        //public async Task Consume(ConsumeContext<AuditEvent> context)
        //{
        //    var msg = context.Message;
        //    // payload is JSON string -> try to map common fields
        //    var raw = msg.Payload ?? "{}";
        //    var parsed = JsonDocument.Parse(raw).RootElement;

        //    var log = new AuditLog
        //    {
        //        EventId = msg.EventId,
        //        EventType = msg.EventType,
        //        ServiceName = msg.ServiceName,
        //        RawPayload = raw,
        //        Timestamp = msg.OccurredAt,
        //        // extracted fields (best-effort)
        //        EntityName = parsed.TryGetProperty("entity", out var e) ? e.GetString() ?? "" : "",
        //        ActionType = parsed.TryGetProperty("action", out var a) ? a.GetString() ?? "" : "",
        //        UserId = parsed.TryGetProperty("userId", out var u) ? u.GetString() ?? "" : "",
        //        PrimaryKey = parsed.TryGetProperty("primaryKey", out var p) ? p.GetString() ?? "" : "",
        //        OldValues = parsed.TryGetProperty("oldValues", out var ov) ? ov.GetRawText() : null,
        //        NewValues = parsed.TryGetProperty("newValues", out var nv) ? nv.GetRawText() : null
        //    };

        //    await _store.AppendAsync(log, context.CancellationToken);
        //}

        public async Task Consume(ConsumeContext<IAuditLogCreated> context)
        {
            var msg = context.Message;

            var log = new AuditLog
            {
                EventId = msg.EventId,
                EventType = $"{msg.EntityName}.{msg.ActionType}", //nameof(IAuditLogCreated),
                ServiceName = msg.ServiceName,
                RawPayload = JsonSerializer.Serialize(msg),
                Timestamp = msg.Timestamp,
                EntityName = msg.EntityName,
                ActionType = msg.ActionType,
                UserId = msg.UserId,
                PrimaryKey = msg.PrimaryKey,
                //OldValues = msg.OldValues,
                //NewValues = msg.NewValues
            };

            await _store.AppendAsync(log, context.CancellationToken);
        }
    }
}
