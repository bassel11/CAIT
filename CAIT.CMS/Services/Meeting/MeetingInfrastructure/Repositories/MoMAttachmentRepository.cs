using MeetingCore.Entities;
using MeetingCore.Repositories;
using MeetingInfrastructure.Data;

namespace MeetingInfrastructure.Repositories
{
    public class MoMAttachmentRepository : RepositoryBase<MoMAttachment>, IMoMAttachmentRepository
    {
        public MoMAttachmentRepository(MeetingDbContext dbContext) : base(dbContext)
        {
        }
    }
}
