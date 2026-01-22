using BuildingBlocks.Shared.CQRS;
using BuildingBlocks.Shared.Wrappers;

namespace MeetingApplication.Features.MoMs.Commands.Models
{
    public record ApproveMoMCommand(Guid MeetingId)
        : ICommand<Result>;
}
