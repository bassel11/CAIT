using MeetingCore.Entities;

namespace MeetingCore.Repositories
{
    public interface IMoMRepository : IAsyncRepository<MinutesOfMeeting>
    {
        Task<MinutesOfMeeting?> GetByMeetingIdAsync(Guid meetingId, CancellationToken ct);
        Task<List<MinutesOfMeeting>> GetByMeetingsIdAsync(Guid meetingId, CancellationToken ct);
        Task AddVersionAsync(MinutesVersion version, CancellationToken ct = default);


    }
}
