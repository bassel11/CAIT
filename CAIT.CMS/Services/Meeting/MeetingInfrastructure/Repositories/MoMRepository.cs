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

        public async Task AddVersionAsync(MinutesVersion version, CancellationToken ct = default)
        {
            await _dbContext.MinutesVersions.AddAsync(version, ct);
        }


        public async Task UpdateMoMAsync(MinutesOfMeeting newData)
        {
            var existing = await _dbContext.Minutes
                .Include(x => x.MoMAttachments)
                .Include(x => x.Versions)
                .FirstOrDefaultAsync(x => x.Id == newData.Id);

            if (existing == null)
                throw new Exception("MinutesOfMeeting not found.");

            // تحديث الحقول الأساسية فقط
            existing.Status = newData.Status;
            existing.ApprovedAt = newData.ApprovedAt;
            existing.ApprovedBy = newData.ApprovedBy;
            existing.UpdatedAt = newData.UpdatedAt;

            // إضافة المرفقات الجديدة فقط
            foreach (var att in newData.MoMAttachments)
            {
                if (!existing.MoMAttachments.Any(x => x.Id == att.Id))
                    existing.MoMAttachments.Add(att);
            }

            // لا نستخدم Update(entity)
            // EF Core يتتبع existing تلقائياً
        }
        public async Task<MinutesOfMeeting?> GetMoMByIdAsync(Guid id)
        {
            return await _dbContext.Minutes
                .Include(m => m.Versions)
                .Include(m => m.MoMAttachments)
                .FirstOrDefaultAsync(m => m.Id == id);
        }
    }
}

