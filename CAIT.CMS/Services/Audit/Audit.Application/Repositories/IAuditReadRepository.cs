using Audit.Application.DTOs;

namespace Audit.Application.Repositories
{
    public interface IAuditReadRepository
    {
        Task<AuditQueryResult> SearchAsync(AuditQueryParams query);
    }
}
