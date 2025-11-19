using CommitteeCore.Entities;

namespace CommitteeCore.Repositories
{
    public interface IStatusHistoryRepository : IAsyncRepository<CommitteeStatusHistory>
    {
        Task<IEnumerable<CommitteeStatusHistory>> GetByCommitteeIdAsync(Guid committeeId);
    }
}
