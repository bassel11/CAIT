using MeetingCore.Entities;
using MeetingCore.Enums;
using MeetingCore.Repositories;
using MeetingInfrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace MeetingInfrastructure.Repositories
{
    public class AttendanceRepository : RepositoryBase<Attendance>, IAttendanceRepository
    {
        public AttendanceRepository(MeetingDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<Attendance?> GetByMeetingAndMemberAsync(Guid meetingId, Guid memberId, CancellationToken ct = default)
        {
            return await _dbContext.Attendance
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.MeetingId == meetingId && a.MemberId == memberId, ct);
        }

        public async Task<List<Attendance>> GetByMeetingAndMembersAsync(Guid meetingId, IEnumerable<Guid> memberIds, CancellationToken ct = default)
        {
            return await _dbContext.Attendance
                .AsNoTracking()
                .Where(a => a.MeetingId == meetingId && memberIds.Contains(a.MemberId))
                .ToListAsync(ct);
        }

        public async Task<List<Attendance>> GetByMeetingAsync(Guid meetingId, CancellationToken ct = default)
        {
            return await _dbContext.Attendance
                .AsNoTracking() // أسرع للقراءة فقط
                .Where(a => a.MeetingId == meetingId)
                .ToListAsync(ct);
        }

        public async Task<int> CountPresentMembersAsync(Guid meetingId, CancellationToken ct)
        {
            return await _dbContext.Attendance
                .Where(a =>
                    a.MeetingId == meetingId &&
                    (a.AttendanceStatus == AttendanceStatus.Present ||
                     a.AttendanceStatus == AttendanceStatus.Remote))
                .Select(a => a.MemberId)
                .Distinct()
                .CountAsync(ct);
        }

    }
}
