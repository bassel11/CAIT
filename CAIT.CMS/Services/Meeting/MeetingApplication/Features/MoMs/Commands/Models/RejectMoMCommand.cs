using BuildingBlocks.Shared.CQRS;
using BuildingBlocks.Shared.Wrappers;

namespace MeetingApplication.Features.MoMs.Commands.Models
{
    public record RejectMoMCommand(Guid MeetingId, string Reason)
        : ICommand<Result>;
}
