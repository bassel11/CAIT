using MeetingCore.Entities;
using MeetingCore.Repositories;
using MeetingInfrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace MeetingInfrastructure.Repositories
{
    public class MeetingRepository : RepositoryBase<Meeting>, IMeetingRepository
    {
        public MeetingRepository(MeetingDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<List<Meeting>> GetByCommitteeIdAsync(Guid committeeId, CancellationToken cancellationToken)
        {
            return await _dbContext.Meetings
                .AsNoTracking()                            // أفضل للأداء
                .Where(m => m.CommitteeId == committeeId)  // يتم تطبيقه داخل SQL
                .OrderByDescending(m => m.CreatedAt)       // فرز سريع في DB
                .ToListAsync(cancellationToken);
        }

    }
}
