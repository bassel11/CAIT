using Audit.Application.DTOs;
using Audit.Application.Repositories;

namespace Audit.Application.Services
{
    public class AuditQueryService : IAuditQueryService
    {
        private readonly IAuditReadRepository _repo;

        public AuditQueryService(IAuditReadRepository repo)
        {
            _repo = repo;
        }

        public async Task<AuditQueryResult> QueryAsync(AuditQueryParams q)
            => await _repo.SearchAsync(q);
    }
}
