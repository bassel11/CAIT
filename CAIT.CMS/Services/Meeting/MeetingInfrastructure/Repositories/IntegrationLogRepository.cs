using MeetingCore.Entities;
using MeetingCore.Repositories;
using MeetingInfrastructure.Data;

namespace MeetingInfrastructure.Repositories
{
    public class IntegrationLogRepository : RepositoryBase<MeetingIntegrationLog>, IIntegrationLogRepository
    {
        public IntegrationLogRepository(MeetingDbContext dbContext) : base(dbContext)
        {
        }
        public Task<MeetingIntegrationLog> AddLogAsync(MeetingIntegrationLog entity)
        {
            _dbContext.Set<MeetingIntegrationLog>().Add(entity);
            return Task.FromResult(entity);
        }
    }
}
