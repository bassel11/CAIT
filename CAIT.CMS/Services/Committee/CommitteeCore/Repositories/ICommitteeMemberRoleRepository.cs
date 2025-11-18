using CommitteeCore.Entities;

namespace CommitteeCore.Repositories
{
    public interface ICommitteeMemberRoleRepository : IAsyncRepository<CommitteeMemberRole>
    {
        Task<List<CommitteeMemberRole>> GetRolesByMemberIdAsync(Guid committeeMemberId);
        Task AddRolesAsync(IEnumerable<CommitteeMemberRole> roles);
        Task RemoveRolesAsync(IEnumerable<CommitteeMemberRole> roles);
        Task<bool> RoleExistsAsync(Guid committeeMemberId, Guid roleId);
    }
}
