using Audit.Application.DTOs;

namespace Audit.Application.Services
{
    public interface IAuditQueryNewService
    {
        Task<List<AuditHistoryDto>> GetHistoryAsync(string entityName, string primaryKey);
    }
}
