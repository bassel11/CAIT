using MeetingCore.Entities;
using MeetingCore.Repositories;
using MeetingInfrastructure.Data;

namespace MeetingInfrastructure.Repositories
{
    public class MeetingNotificationRepository : RepositoryBase<MeetingNotification>, IMeetingNotificationRepository
    {
        public MeetingNotificationRepository(MeetingDbContext dbContext) : base(dbContext)
        {
        }
    }
}
