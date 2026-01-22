using BuildingBlocks.Shared.CQRS;
using BuildingBlocks.Shared.Wrappers;

namespace MeetingApplication.Features.MoMs.Commands.Models
{
    public record UpdateMoMContentCommand(Guid MeetingId, string Content)
        : ICommand<Result>;
}
