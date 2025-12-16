using BuildingBlocks.Shared.CQRS;
using DecisionApplication.Dtos;

namespace DecisionApplication.Decisions.Queries.GetDecisionsByMeeting
{
    public record GetDecisionsByMeetingQuery(Guid MeetingId) : IQuery<GetDecisionsByMeetingResult>;
    public record GetDecisionsByMeetingResult(IEnumerable<DecisionDto> Decisions);
}
