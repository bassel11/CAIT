using MeetingApplication.Integrations;
using MeetingCore.Entities;
using MeetingInfrastructure.Data;

namespace MeetingInfrastructure.Audit
{
    public class AuditService : IAuditService
    {
        private readonly MeetingDbContext _db;
        public AuditService(MeetingDbContext db) => _db = db;
        public async Task RecordAsync(AuditLog log, CancellationToken ct = default)
        {
            _db.AuditLogs.Add(log);
            await _db.SaveChangesAsync(ct);
        }
    }

}
