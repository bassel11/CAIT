using BuildingBlocks.Shared.CQRS;
using MeetingApplication.Features.MoMDecisionDrafts.Queries.Results;

namespace MeetingApplication.Features.MoMDecisionDrafts.Queries.Models
{
    public class GetMoMDecisionsQuery
        : PaginationRequest, IQuery<PaginatedResult<MoMDecisionResponse>>
    {
        public Guid MeetingId { get; set; }
    }
}
