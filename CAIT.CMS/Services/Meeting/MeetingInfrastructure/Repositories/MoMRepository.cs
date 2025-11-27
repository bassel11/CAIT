using MeetingCore.Entities;
using MeetingCore.Repositories;
using MeetingInfrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace MeetingInfrastructure.Repositories
{
    public class MoMRepository : RepositoryBase<MinutesOfMeeting>, IMoMRepository
    {
        public MoMRepository(MeetingDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<MinutesOfMeeting?> GetByMeetingIdAsync(Guid meetingId, CancellationToken ct)
        {
            return await _dbContext.Minutes
                .AsNoTracking() // تحسين الأداء لأننا لا نحتاج للتتبع
                .FirstOrDefaultAsync(m => m.MeetingId == meetingId, ct);

        }
        public async Task<List<MinutesOfMeeting>> GetByMeetingsIdAsync(Guid meetingId, CancellationToken ct)
        {
            return await _dbContext.Minutes
                .AsNoTracking()
                .Where(m => m.MeetingId == meetingId)
                .OrderByDescending(m => m.CreatedAt)
                .ToListAsync(ct);
        }


    }
}
