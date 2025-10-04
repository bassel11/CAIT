using CommitteeCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommitteeCore.Repositories
{
    public  interface  ICommitteeRepository : IAsyncRepository<Committee>
    {
        Task<IEnumerable<Committee>> GetCommitteesById(Guid Id);
    }
}
