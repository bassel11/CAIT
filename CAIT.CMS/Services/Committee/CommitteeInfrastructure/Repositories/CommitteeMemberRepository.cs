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
    }
}
