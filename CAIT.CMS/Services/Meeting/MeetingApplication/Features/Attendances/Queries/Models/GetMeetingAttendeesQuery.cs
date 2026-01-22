using BuildingBlocks.Shared.CQRS;
using MeetingApplication.Features.Attendances.Queries.Results;

namespace MeetingApplication.Features.Attendances.Queries.Models
{
    public class GetMeetingAttendeesQuery : PaginationRequest, IQuery<PaginatedResult<AttendanceResponse>>
    {
        public Guid MeetingId { get; set; }
    }
}
