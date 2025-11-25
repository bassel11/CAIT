using MeetingCore.Entities;

namespace MeetingCore.Repositories
{
    public interface IMeetingRepository : IAsyncRepository<Meeting>
    {
        Task<List<Meeting>> GetByCommitteeIdAsync(Guid committeeId, CancellationToken cancellationToken);
    }
}
