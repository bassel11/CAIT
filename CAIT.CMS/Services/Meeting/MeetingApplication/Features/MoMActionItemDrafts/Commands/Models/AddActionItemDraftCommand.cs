using BuildingBlocks.Shared.CQRS;
using BuildingBlocks.Shared.Wrappers;

namespace MeetingApplication.Features.MoMActionItemDrafts.Commands.Models
{
    public record AddActionItemDraftCommand(Guid MeetingId, string TaskTitle, Guid? AssigneeId, DateTime? DueDate)
        : ICommand<Result>;
}
