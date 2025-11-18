using CommitteeCore.Entities;
using CommitteeCore.Repositories;
using CommitteeInfrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CommitteeInfrastructure.Repositories
{
    public class CommitteeMemberRepository : RepositoryBase<CommitteeMember>, ICommitteeMemberRepository
    {
        public CommitteeMemberRepository(CommitteeContext dbContext) : base(dbContext)
        {
        }

        public async Task<IEnumerable<CommitteeMember>> GetCommitteesMembersByCommId(Guid CommitteeId)
        {
            var commMembs = await _dbContext.CommitteeMembers
                .Where(o => o.CommitteeId == CommitteeId)
                .ToListAsync();
            return commMembs;
        }
        public async Task<bool> IsMemberExistsAsync(Guid committeeId, Guid userId)
        {
            return await _dbContext.CommitteeMembers
                .AnyAsync(c => c.CommitteeId == committeeId && c.UserId == userId);
        }

        public async Task AddRangeAsync(IEnumerable<CommitteeMember> members)
        {
            await _dbContext.CommitteeMembers.AddRangeAsync(members);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<CommitteeMember?> GetByCommitteeAndUserAsync(Guid committeeId, Guid MemberId)
        {
            return await _dbContext.CommitteeMembers
                .Include(cm => cm.CommitteeMemberRoles) // إذا أردت حذف الـ Roles تلقائياً
                .FirstOrDefaultAsync(cm => cm.CommitteeId == committeeId && cm.Id == MemberId);

            //return await _dbContext.CommitteeMembers
            //    .FirstOrDefaultAsync(cm => cm.CommitteeId == committeeId && cm.Id == MemberId);
        }

        public async Task RemoveRangeAsync(IEnumerable<CommitteeMember> members)
        {
            _dbContext.CommitteeMembers.RemoveRange(members);
            await _dbContext.SaveChangesAsync();
        }


    }
}
