using MeetingCore.Entities;

namespace MeetingCore.Repositories
{
    public interface IAgendaRepository : IAsyncRepository<AgendaItem>
    {
        Task<List<AgendaItem>> GetAgendaItemsByMeetingIdAsync(Guid meetingId, CancellationToken ct);
    }
}
