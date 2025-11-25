using CommitteeCore.Entities;
using CommitteeCore.Repositories;
using CommitteeInfrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CommitteeInfrastructure.Repositories
{
    public class CommitteeQuorumRuleRepository : RepositoryBase<CommitteeQuorumRule>, ICommitteeQuorumRuleRepository
    {
        public CommitteeQuorumRuleRepository(CommitteeContext dbContext) : base(dbContext)
        {
        }

        public async Task<bool> ExistsByCommitteeId(Guid committeeId)
        => await _dbContext.CommitteeQuorumRules.AnyAsync(r => r.CommitteeId == committeeId);
    }
}
