using DecisionCore.Enums;

namespace DecisionApplication.Decisions.Queries.GetDecisionsByType
{
    public record GetDecisionsByTypeQuery(DecisionType Type) : IQuery<GetDecisionsByTypeResult>;

    public record GetDecisionsByTypeResult(IEnumerable<DecisionDto> Decisions);
}
