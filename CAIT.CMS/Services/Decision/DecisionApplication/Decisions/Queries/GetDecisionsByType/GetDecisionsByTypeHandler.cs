namespace DecisionApplication.Decisions.Queries.GetDecisionsByType
{
    public class GetDecisionsByTypeHandler(IApplicationDbContext dbContext)
        : IQueryHandler<GetDecisionsByTypeQuery, GetDecisionsByTypeResult>
    {
        public async Task<GetDecisionsByTypeResult> Handle(GetDecisionsByTypeQuery query, CancellationToken cancellationToken)
        {
            var decisions = await dbContext.Decisions
                .Include(d => d.Votes)
                .AsNoTracking()
                .Where(d => d.Type == query.Type)
                .OrderBy(d => d.CreatedAt)
                .ToListAsync(cancellationToken);

            return new GetDecisionsByTypeResult(decisions.ToDecisionDtoList());
        }
    }
}
