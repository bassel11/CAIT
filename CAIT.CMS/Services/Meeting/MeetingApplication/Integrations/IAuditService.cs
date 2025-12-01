using MeetingCore.Entities;

namespace MeetingApplication.Integrations
{
    public interface IAuditService
    {
        Task RecordAsync(AuditLog log, CancellationToken ct = default);
    }
}
