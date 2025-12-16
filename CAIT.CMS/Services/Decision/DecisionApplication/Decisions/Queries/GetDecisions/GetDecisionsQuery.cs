using BuildingBlocks.Shared.CQRS;
using BuildingBlocks.Shared.Pagination;
using DecisionApplication.Dtos;

namespace DecisionApplication.Decisions.Queries.GetDecisions
{
    public record GetDecisionsQuery(PaginationRequest PaginationRequest) : IQuery<GetDecisionsResult>;

    public record GetDecisionsResult(PaginatedResult<DecisionDto> Decisions);
}
