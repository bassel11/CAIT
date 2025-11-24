using MeetingCore.Entities;
using MeetingCore.Repositories;
using MeetingInfrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace MeetingInfrastructure.Repositories
{
    public class AgendaRepository : RepositoryBase<AgendaItem>, IAgendaRepository
    {
        public AgendaRepository(MeetingDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<List<AgendaItem>> GetAgendaItemsByMeetingIdAsync(Guid meetingId, CancellationToken ct)
        {
            return await _dbContext.AgendaItems
                .Where(a => a.MeetingId == meetingId)
                .OrderBy(a => a.SortOrder)
                .AsNoTracking()
                .ToListAsync(ct);
        }
    }
}
