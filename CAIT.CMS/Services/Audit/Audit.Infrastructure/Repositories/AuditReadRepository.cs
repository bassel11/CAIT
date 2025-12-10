using Audit.Application.DTOs;
using Audit.Application.Repositories;
using Audit.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Audit.Infrastructure.Repositories
{
    public class AuditReadRepository : IAuditReadRepository
    {
        private readonly AuditDbContext _db;

        public AuditReadRepository(AuditDbContext db)
        {
            _db = db;
        }

        public async Task<AuditQueryResult> SearchAsync(AuditQueryParams q)
        {
            var query = _db.AuditLogs.AsNoTracking().AsQueryable();

            if (!string.IsNullOrEmpty(q.ServiceName))
                query = query.Where(x => x.ServiceName == q.ServiceName);

            if (!string.IsNullOrEmpty(q.EntityName))
                query = query.Where(x => x.EntityName == q.EntityName);

            if (!string.IsNullOrEmpty(q.UserId))
                query = query.Where(x => x.UserId == q.UserId);

            if (!string.IsNullOrEmpty(q.ActionType))
                query = query.Where(x => x.ActionType == q.ActionType);

            if (!string.IsNullOrEmpty(q.PrimaryKey))
                query = query.Where(x => x.PrimaryKey == q.PrimaryKey);

            if (q.From.HasValue)
                query = query.Where(x => x.Timestamp >= q.From.Value);

            if (q.To.HasValue)
                query = query.Where(x => x.Timestamp <= q.To.Value);

            query = q.SortBy switch
            {
                "timestamp_asc" => query.OrderBy(x => x.Timestamp),
                _ => query.OrderByDescending(x => x.Timestamp)
            };

            var total = await query.CountAsync();
            var items = await query.Skip(q.Skip).Take(q.Take).ToListAsync();

            return new AuditQueryResult(total, items);
        }
    }
}
