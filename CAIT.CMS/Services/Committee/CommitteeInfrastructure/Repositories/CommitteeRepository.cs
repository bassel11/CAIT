using CommitteeCore.Entities;
using CommitteeCore.Repositories;
using CommitteeInfrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommitteeInfrastructure.Repositories
{
    public class CommitteeRepository : RepositoryBase<Committee>, ICommitteeRepository
    {
        public CommitteeRepository(CommitteeContext dbContext) : base(dbContext)
        {

        }

        public async Task<IEnumerable<Committee>> GetCommitteesById(Guid id)
        {
            var orderList = await _dbContext.Committees
                .Where(o => o.Id == id)
                .ToListAsync();
            return orderList;
        }

    }
}
