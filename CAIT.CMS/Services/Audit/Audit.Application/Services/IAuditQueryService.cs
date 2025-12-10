using Audit.Application.DTOs;

namespace Audit.Application.Services
{
    public interface IAuditQueryService
    {
        Task<AuditQueryResult> QueryAsync(AuditQueryParams query);
    }
}
