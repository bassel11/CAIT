using BuildingBlocks.Shared.CQRS;
using MeetingApplication.Features.MoMActionItemDrafts.Queries.Results;

namespace MeetingApplication.Features.MoMActionItemDrafts.Queries.Models
{
    public class GetMoMActionItemsQuery
        : PaginationRequest, IQuery<PaginatedResult<MoMActionItemResponse>>
    {
        public Guid MeetingId { get; set; }
    }
}
