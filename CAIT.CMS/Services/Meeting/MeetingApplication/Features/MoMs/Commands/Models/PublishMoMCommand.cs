using BuildingBlocks.Shared.CQRS;
using BuildingBlocks.Shared.Wrappers;

namespace MeetingApplication.Features.MoMs.Commands.Models
{
    public record PublishMoMCommand(Guid MeetingId)
        : ICommand<Result>;
}
