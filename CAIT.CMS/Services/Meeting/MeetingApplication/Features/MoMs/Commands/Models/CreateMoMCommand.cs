using BuildingBlocks.Shared.CQRS;
using BuildingBlocks.Shared.Wrappers;

namespace MeetingApplication.Features.MoMs.Commands.Models
{
    public record CreateMoMCommand(Guid MeetingId, string InitialContent)
        : ICommand<Result<Guid>>;
}
