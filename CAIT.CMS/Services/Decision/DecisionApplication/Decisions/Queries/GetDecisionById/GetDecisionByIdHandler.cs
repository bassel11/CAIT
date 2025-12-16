namespace DecisionApplication.Decisions.Queries.GetDecisionById
{
    using Microsoft.EntityFrameworkCore;

    namespace DecisionApplication.Decisions.Queries.GetDecisionById
    {
        public class GetDecisionByIdHandler(IApplicationDbContext dbContext)
            : IQueryHandler<GetDecisionByIdQuery, DecisionDto>
        {
            public async Task<DecisionDto> Handle(GetDecisionByIdQuery query, CancellationToken cancellationToken)
            {
                var decisionId = DecisionCore.ValueObjects.DecisionId.Of(query.Id);

                var decision = await dbContext.Decisions
                    .Include(d => d.Votes)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(d => d.Id == decisionId, cancellationToken);

                if (decision == null)
                    throw new KeyNotFoundException($"Decision with Id {query.Id} not found.");

                return decision.ToDecisionDto(); // استخدام امتداد التحويل إلى DTO
            }
        }
    }
}
