using CommitteeCore.Entities;

namespace CommitteeCore.Repositories
{
    public interface ICommitteeQuorumRuleRepository : IAsyncRepository<CommitteeQuorumRule>
    {
        Task<bool> ExistsByCommitteeId(Guid committeeId);
        Task<CommitteeQuorumRule?> GetByCommitteeIdAsync(Guid committeeId);
    }
}
