namespace MeetingApplication.Features.AgendaItems.Queries.Models
{
    using BuildingBlocks.Shared.CQRS;
    using MeetingApplication.Common;
    using MeetingApplication.Features.AgendaItems.Queries.Results;
    using MeetingApplication.Wrappers; // PaginatedResult, PaginationRequest

    public class GetAgendaItemsQuery : PaginationRequest, IQuery<PaginatedResult<AgendaItemResponse>>
    {
        public Guid MeetingId { get; set; }
    }
}
