using BuildingBlocks.Shared.CQRS;
using MeetingApplication.Features.Attendances.Queries.Results;

namespace MeetingApplication.Features.Attendances.Queries.Models
{
    public class GetMemberAttendanceHistoryQuery : PaginationRequest, IQuery<PaginatedResult<AttendanceResponse>>
    {
        public Guid MemberId { get; set; }
    }
}
