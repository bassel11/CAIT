using BuildingBlocks.Shared.CQRS;
using TaskApplication.Dtos;

namespace TaskApplication.Features.Attachments.Queries.GetAttachmentHistory
{
    public record GetAttachmentHistoryQuery(Guid TaskId, Guid CurrentAttachmentId) : IQuery<List<TaskAttachmentDto>>;
}
