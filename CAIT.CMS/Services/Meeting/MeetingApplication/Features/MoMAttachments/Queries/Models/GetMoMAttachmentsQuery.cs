using BuildingBlocks.Shared.CQRS;
using BuildingBlocks.Shared.Wrappers;

namespace MeetingApplication.Features.MoMAttachments.Queries.Models
{
    // DTO
    public class MoMAttachmentDto
    {
        public Guid Id { get; set; }
        public string FileName { get; set; } = default!;
        public string ContentType { get; set; } = default!;
        public long Size { get; set; }
        public DateTime UploadedAt { get; set; }
        public Guid UploadedBy { get; set; }
        public string DownloadUrl { get; set; } = default!; // رابط التحميل
    }

    // Query
    public record GetMoMAttachmentsQuery(Guid MeetingId)
        : IQuery<Result<List<MoMAttachmentDto>>>;
}
