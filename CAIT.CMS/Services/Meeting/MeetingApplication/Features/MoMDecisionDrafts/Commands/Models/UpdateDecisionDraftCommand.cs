using BuildingBlocks.Shared.CQRS;
using BuildingBlocks.Shared.Wrappers;

namespace MeetingApplication.Features.MoMDecisionDrafts.Commands.Models
{
    public record UpdateDecisionDraftCommand(
    Guid MeetingId,
    Guid DecisionId,
    string Title,
    string Text
) : ICommand<Result>;
}
