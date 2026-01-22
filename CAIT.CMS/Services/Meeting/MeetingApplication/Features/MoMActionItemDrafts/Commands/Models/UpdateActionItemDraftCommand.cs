using BuildingBlocks.Shared.CQRS;
using BuildingBlocks.Shared.Wrappers;

namespace MeetingApplication.Features.MoMActionItemDrafts.Commands.Models
{
    public record UpdateActionItemDraftCommand(
    Guid MeetingId,
    Guid ActionItemId,
    string TaskTitle,
    Guid? AssigneeId,
    DateTime? DueDate
) : ICommand<Result>;
}
