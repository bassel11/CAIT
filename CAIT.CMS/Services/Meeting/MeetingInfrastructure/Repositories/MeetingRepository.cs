using MeetingCore.Entities;
using MeetingCore.Repositories;
using MeetingInfrastructure.Data;

namespace MeetingInfrastructure.Repositories
{
    public class MeetingRepository : RepositoryBase<Meeting>, IMeetingRepository
    {
        public MeetingRepository(MeetingDbContext dbContext) : base(dbContext)
        {
        }
    }
}
