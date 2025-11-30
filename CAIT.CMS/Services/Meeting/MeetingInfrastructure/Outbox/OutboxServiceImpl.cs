using MeetingApplication.Interfaces.Integrations;
using MeetingCore.Entities;
using MeetingInfrastructure.Data;

namespace MeetingInfrastructure.Outbox
{
    public class OutboxServiceImpl : IOutboxService
    {
        private readonly MeetingDbContext _db;


        public OutboxServiceImpl(MeetingDbContext db) { _db = db; }


        public async Task EnqueueAsync(OutboxMessage message, CancellationToken ct = default)
        {
            _db.OutboxMessages.Add(message);
            await _db.SaveChangesAsync(ct);
        }
    }
}
