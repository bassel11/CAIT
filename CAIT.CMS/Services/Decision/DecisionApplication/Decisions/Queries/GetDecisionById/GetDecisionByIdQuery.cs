namespace DecisionApplication.Decisions.Queries.GetDecisionById
{
    public record GetDecisionByIdQuery(Guid Id) : IQuery<DecisionDto>;
}
