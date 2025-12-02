using MeetingCore.Entities;
using MeetingCore.Repositories;
using MeetingInfrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace MeetingInfrastructure.Repositories
{
    public class MoMAttachmentRepository : RepositoryBase<MoMAttachment>, IMoMAttachmentRepository
    {
        public MoMAttachmentRepository(MeetingDbContext dbContext) : base(dbContext)
        {
        }
        public async Task<MoMAttachment?> GetPdfByMoMIdAsync(Guid momId, CancellationToken ct)
        {
            return await _dbContext.MoMAttachments
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.MoMId == momId && a.ContentType == "application/pdf", ct);
        }

        public Task<MoMAttachment> AddMoMAttachmentAsync(MoMAttachment entity)
        {
            _dbContext.MoMAttachments.Add(entity);
            //await _dbContext.SaveChangesAsync();
            return Task.FromResult(entity);
        }
    }
}
