using MeetingCore.Entities;

namespace MeetingCore.Repositories
{
    public interface IAttendanceRepository : IAsyncRepository<Attendance>
    {
        Task<bool> ExistsAsync(Guid meetingId, CancellationToken ct = default);

        Task<Attendance?> GetByMeetingAndMemberAsync(Guid meetingId, Guid memberId, CancellationToken ct = default);

        Task<List<Attendance>> GetByMeetingAndMembersAsync(Guid meetingId, IEnumerable<Guid> memberIds, CancellationToken ct = default);

        Task<List<Attendance>> GetByMeetingAsync(Guid meetingId, CancellationToken ct = default);

        Task<int> CountPresentMembersAsync(Guid meetingId, CancellationToken ct = default);

        Task BulkAddAsync(IEnumerable<Attendance> attendances, CancellationToken ct = default);

        void BulkUpdate(IEnumerable<Attendance> attendances);

        Task DeleteAsync(Attendance attendance);

        Task SaveChangesAsync(CancellationToken ct = default);

    }
}
