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

        // تحقق سريع من وجود الاجتماع
        public async Task<bool> ExistsAsync(Guid meetingId, CancellationToken ct = default)
        {
            return await _dbContext.Meetings
                .AsNoTracking()
                .AnyAsync(m => m.Id == meetingId, ct);
        }

        // جلب سجل لحضور عضو واحد
        public async Task<Attendance?> GetByMeetingAndMemberAsync(Guid meetingId, Guid memberId, CancellationToken ct = default)
        {
            return await _dbContext.Attendance
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.MeetingId == meetingId && a.MemberId == memberId, ct);
        }

        // جلب سجلات أعضاء محددين في اجتماع
        public async Task<List<Attendance>> GetByMeetingAndMembersAsync(Guid meetingId, IEnumerable<Guid> memberIds, CancellationToken ct = default)
        {
            return await _dbContext.Attendance
                .AsNoTracking()
                .Where(a => a.MeetingId == meetingId && memberIds.Contains(a.MemberId))
                .ToListAsync(ct);
        }

        // جلب كل الحضور لاجتماع معين
        public async Task<List<Attendance>> GetByMeetingAsync(Guid meetingId, CancellationToken ct = default)
        {
            return await _dbContext.Attendance
                .AsNoTracking()
                .Where(a => a.MeetingId == meetingId)
                .ToListAsync(ct);
        }

        // عد الأعضاء الحاضرين أو عن بعد
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

        // -----------------------------------------
        // Bulk Add
        // -----------------------------------------
        public async Task BulkAddAsync(IEnumerable<Attendance> attendances, CancellationToken ct = default)
        {
            await _dbContext.Attendance.AddRangeAsync(attendances, ct);
        }

        // -----------------------------------------
        // Bulk Update
        // -----------------------------------------
        public void BulkUpdate(IEnumerable<Attendance> attendances)
        {
            // EF Core يتعقب التغييرات تلقائياً إذا Entities متصلة بالـ DbContext
            // نستخدم Attach لتأكيد تتبع الكيانات
            foreach (var a in attendances)
            {
                _dbContext.Attendance.Attach(a);
                _dbContext.Entry(a).Property(x => x.AttendanceStatus).IsModified = true;
                _dbContext.Entry(a).Property(x => x.Timestamp).IsModified = true;
            }
        }

        // -----------------------------------------
        // Delete
        // -----------------------------------------
        public async Task DeleteAsync(Attendance attendance)
        {
            _dbContext.Attendance.Remove(attendance);
        }

        // -----------------------------------------
        // Save Changes
        // -----------------------------------------
        public async Task SaveChangesAsync(CancellationToken ct = default)
        {
            await _dbContext.SaveChangesAsync(ct);
        }
    }
}
