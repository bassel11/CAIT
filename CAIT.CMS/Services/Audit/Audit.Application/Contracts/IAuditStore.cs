using Audit.Domain.Entities;

namespace Audit.Application.Contracts
{
    public interface IAuditStore
    {
        Task AppendAsync(AuditLog log, CancellationToken ct = default);
    }
}
