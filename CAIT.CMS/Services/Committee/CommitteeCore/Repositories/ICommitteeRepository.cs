using CommitteeCore.Entities;

namespace CommitteeCore.Repositories
{
    public interface ICommitteeRepository : IAsyncRepository<Committee>
    {
        //Task<IEnumerable<Committee>> GetCommitteesById(Guid Id);
    }
}
