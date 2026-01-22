using BuildingBlocks.Shared.CQRS;
using BuildingBlocks.Shared.Wrappers;

namespace MeetingApplication.Features.MoMActionItemDrafts.Commands.Models
{
    public record RemoveActionItemDraftCommand(Guid MeetingId, Guid ActionItemId)
        : ICommand<Result>;
}
