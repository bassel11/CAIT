using BuildingBlocks.Shared.CQRS;
using BuildingBlocks.Shared.Wrappers;

namespace MeetingApplication.Features.MoMDecisionDrafts.Commands.Models
{
    public record AddDecisionDraftCommand(Guid MeetingId, string Title, string Text)
        : ICommand<Result>;
}
