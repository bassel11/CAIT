using CommitteeCore.Entities;

namespace CommitteeCore.Repositories
{
    public interface ICommitteeMemberRepository : IAsyncRepository<CommitteeMember>
    {
        Task<IEnumerable<CommitteeMember>> GetCommitteesMembersByCommId(Guid CommitteeId);
    }
}
