using MeetingApplication.Integrations;
using MeetingCore.Entities;
using MeetingInfrastructure.Data;
using System.Text.Json;

namespace MeetingInfrastructure.Outbox
{
    public class OutboxService : IOutboxService
    {
        private readonly MeetingDbContext _db;
        private readonly JsonSerializerOptions _jsonOptions =
            new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

        public OutboxService(MeetingDbContext db)
        {
            _db = db;
        }

        public async Task EnqueueAsync(string type, object payload, CancellationToken ct)
        {
            var entity = new OutboxMessage
            {
                Id = Guid.NewGuid(),
                Type = type,
                Payload = JsonSerializer.Serialize(payload, _jsonOptions),
                OccurredAt = DateTime.UtcNow,
                Processed = false
            };

            _db.OutboxMessages.Add(entity);
            // await _db.SaveChangesAsync(ct); commented at 01-12-2025
        }
    }
    //public class OutboxServiceImpl : IOutboxService
    //{
    //    private readonly MeetingDbContext _db;
    //    public OutboxServiceImpl(MeetingDbContext db) { _db = db; }
    //    public async Task EnqueueAsync(OutboxMessage message, CancellationToken ct = default)
    //    {
    //        _db.OutboxMessages.Add(message);
    //        // Important: don't call SaveChanges here if caller will save in same transaction (TransactionBehavior/UnitOfWork handles commit)
    //        await _db.SaveChangesAsync(ct);
    //    }
    //}
}
