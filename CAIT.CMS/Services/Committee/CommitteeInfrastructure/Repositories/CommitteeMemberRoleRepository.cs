using CommitteeCore.Entities;
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

        public async Task<List<Guid>> GetRoleIdsByMemberIdAsync(Guid committeeMemberId)
        {
            return await _dbContext.CommitteeMemberRoles
                .Where(r => r.CommitteeMemberId == committeeMemberId)
                .Select(r => r.RoleId)
                .ToListAsync();
        }

        public async Task AddRolesAsync(IEnumerable<CommitteeMemberRole> roles)
        {
            if (roles == null || !roles.Any()) return;

            await _dbContext.CommitteeMemberRoles.AddRangeAsync(roles);
            await _dbContext.SaveChangesAsync();
        }

        public async Task RemoveRolesAsync(IEnumerable<CommitteeMemberRole> roles)
        {
            if (roles == null || !roles.Any()) return;

            _dbContext.CommitteeMemberRoles.RemoveRange(roles);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<bool> RoleExistsAsync(Guid committeeMemberId, Guid roleId, Guid? excludeId = null)
        {
            var query = _dbContext.CommitteeMemberRoles
                .Where(r => r.CommitteeMemberId == committeeMemberId && r.RoleId == roleId);

            if (excludeId.HasValue)
                query = query.Where(r => r.Id != excludeId.Value);

            return await query.AnyAsync();
        }
    }
}
