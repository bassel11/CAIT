using CommitteeCore.Entities;
using CommitteeCore.Repositories;
using CommitteeInfrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CommitteeInfrastructure.Repositories
{
    public class CommitteeRepository : RepositoryBase<Committee>, ICommitteeRepository
    {
        public CommitteeRepository(CommitteeContext dbContext) : base(dbContext)
        {

        }

        public async Task<IEnumerable<Committee>> GetCommitteesById(Guid id)
        {
            var committeesList = await _dbContext.Committees
            .Where(o => o.Id == id)
            .ToListAsync();
            return committeesList;
        }

    }
}
