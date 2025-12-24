using BuildingBlocks.Shared.CQRS;

namespace TaskApplication.Features.Attachments.Queries.GetAttachment
{
    public record GetAttachmentContentQuery(Guid TaskId, Guid AttachmentId) : IQuery<FileDownloadResponse>;

    public record FileDownloadResponse(Stream Stream, string ContentType, string FileName);
}
