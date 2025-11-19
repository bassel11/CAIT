using CommitteeCore.Entities;
using CommitteeCore.Repositories;
using CommitteeInfrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CommitteeInfrastructure.Repositories
{
    public class StatusHistoryRepository : RepositoryBase<CommitteeStatusHistory>, IStatusHistoryRepository
    {
        public StatusHistoryRepository(CommitteeContext dbContext) : base(dbContext)
        {
        }

        public async Task<IEnumerable<CommitteeStatusHistory>> GetByCommitteeIdAsync(Guid committeeId)
        {
            return await _dbContext.CommitteeStatusHistories
                .Include(h => h.Committee)
                .Include(h => h.OldStatusId)
                .Include(h => h.NewStatusId)
                .Where(h => h.CommitteeId == committeeId)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
