using MeetingCore.Repositories;
using MeetingInfrastructure.Data;

namespace MeetingInfrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly MeetingDbContext _dbContext;

        public UnitOfWork(MeetingDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return _dbContext.SaveChangesAsync(cancellationToken);
        }
    }

}
