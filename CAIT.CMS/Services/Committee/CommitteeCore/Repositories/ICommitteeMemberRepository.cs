using CommitteeCore.Entities;

namespace CommitteeCore.Repositories
{
    public interface ICommitteeMemberRepository : IAsyncRepository<CommitteeMember>
    {
        Task<IEnumerable<CommitteeMember>> GetCommitteesMembersByCommId(Guid CommitteeId);
        Task<bool> IsMemberExistsAsync(Guid committeeId, Guid userId);
        Task AddRangeAsync(IEnumerable<CommitteeMember> members);

        Task<CommitteeMember?> GetByCommitteeAndUserAsync(Guid committeeId, Guid MemberId);
        Task RemoveRangeAsync(IEnumerable<CommitteeMember> members);

    }
}
