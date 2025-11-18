using CommitteeCore.Entities;
using CommitteeCore.Repositories;
using CommitteeInfrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CommitteeInfrastructure.Repositories
{
    public class CommitteeMemberRoleRepository : RepositoryBase<CommitteeMemberRole>, ICommitteeMemberRoleRepository
    {
        public CommitteeMemberRoleRepository(CommitteeContext dbContext) : base(dbContext)
        {
        }

        public async Task<List<CommitteeMemberRole>> GetRolesByMemberIdAsync(Guid committeeMemberId)
        {
            return await _dbContext.CommitteeMemberRoles
                .Where(r => r.CommitteeMemberId == committeeMemberId)
                .ToListAsync();
        }

        public async Task AddRolesAsync(IEnumerable<CommitteeMemberRole> roles)
        {
            await _dbContext.CommitteeMemberRoles.AddRangeAsync(roles);
            await _dbContext.SaveChangesAsync();
        }

        public async Task RemoveRolesAsync(IEnumerable<CommitteeMemberRole> roles)
        {
            _dbContext.CommitteeMemberRoles.RemoveRange(roles);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<bool> RoleExistsAsync(Guid committeeMemberId, Guid roleId)
        {
            return await _dbContext.CommitteeMemberRoles
                .AnyAsync(r => r.CommitteeMemberId == committeeMemberId && r.RoleId == roleId);
        }
    }
}
