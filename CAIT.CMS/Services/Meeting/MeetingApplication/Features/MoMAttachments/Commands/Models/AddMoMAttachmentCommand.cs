using BuildingBlocks.Shared.CQRS;
using BuildingBlocks.Shared.Wrappers;

namespace MeetingApplication.Features.MoMAttachments.Commands.Models
{
    public record AddMoMAttachmentCommand(
        Guid MeetingId,
        string FileName,
        string ContentType,
        long SizeInBytes,
        string StoragePath
    ) : ICommand<Result<Guid>>;
}
