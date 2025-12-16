using BuildingBlocks.Shared.CQRS;
using BuildingBlocks.Shared.Pagination;
using DecisionApplication.Data;
using DecisionApplication.Dtos;
using DecisionApplication.Extensions;
using Microsoft.EntityFrameworkCore;

namespace DecisionApplication.Decisions.Queries.GetDecisions
{
    public class GetDecisionsHandler(IApplicationDbContext dbContext)
        : IQueryHandler<GetDecisionsQuery, GetDecisionsResult>
    {
        public async Task<GetDecisionsResult> Handle(GetDecisionsQuery query, CancellationToken cancellationToken)
        {
            var pageIndex = query.PaginationRequest.PageIndex;
            var pageSize = query.PaginationRequest.PageSize;
            var totalCount = await dbContext.Decisions.LongCountAsync(cancellationToken);

            var decisions = await dbContext.Decisions
                .Include(d => d.Votes)
                .OrderBy(d => d.CreatedAt)
                .Skip(pageSize * pageIndex)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return new GetDecisionsResult(
                new PaginatedResult<DecisionDto>(
                    pageIndex, pageSize, totalCount, decisions.ToDecisionDtoList()));
        }
    }
}
