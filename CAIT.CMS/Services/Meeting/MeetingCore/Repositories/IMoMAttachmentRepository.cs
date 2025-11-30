using MeetingCore.Entities;

namespace MeetingCore.Repositories
{
    public interface IMoMAttachmentRepository : IAsyncRepository<MoMAttachment>
    {
        Task<MoMAttachment?> GetPdfByMoMIdAsync(Guid momId, CancellationToken ct);

    }
}
