using BuildingBlocks.Shared.CQRS;
using BuildingBlocks.Shared.Wrappers;

namespace MeetingApplication.Features.MoMAttachments.Commands.Models
{
    public record RemoveMoMAttachmentCommand(
        Guid MeetingId,
        Guid AttachmentId
    ) : ICommand<Result>;
}
