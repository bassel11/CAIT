using BuildingBlocks.Shared.CQRS;
using BuildingBlocks.Shared.Wrappers;

namespace MeetingApplication.Features.MoMDecisionDrafts.Commands.Models
{
    public record RemoveDecisionDraftCommand(Guid MeetingId, Guid DecisionId) : ICommand<Result>;
}
