using BuildingBlocks.Shared.CQRS;
using MeetingApplication.Features.Meetings.Queries.Results;

namespace MeetingApplication.Features.Meetings.Queries.Models
{
    public class GetMeetingsQuery
        : PaginationRequest, IQuery<PaginatedResult<GetMeetingResponse>>
    {
        public Guid? CommitteeId { get; set; } // فلترة اختيارية
        public string? Status { get; set; }    // فلترة اختيارية
    }
}
