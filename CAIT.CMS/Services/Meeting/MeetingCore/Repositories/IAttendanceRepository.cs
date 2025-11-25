using MeetingCore.Entities;

namespace MeetingCore.Repositories
{
    public interface IAttendanceRepository : IAsyncRepository<Attendance>
    {
        Task<Attendance?> GetByMeetingAndMemberAsync(Guid meetingId, Guid memberId, CancellationToken ct = default);
        Task<List<Attendance>> GetByMeetingAndMembersAsync(Guid meetingId, IEnumerable<Guid> memberIds, CancellationToken ct = default);
        Task<List<Attendance>> GetByMeetingAsync(Guid meetingId, CancellationToken ct = default);
        Task<int> CountPresentMembersAsync(Guid meetingId, CancellationToken ct);

    }
}
