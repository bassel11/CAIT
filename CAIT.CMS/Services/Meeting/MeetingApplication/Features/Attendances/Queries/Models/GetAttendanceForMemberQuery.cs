using BuildingBlocks.Shared.CQRS;
using MeetingApplication.Features.Attendances.Queries.Results;

namespace MeetingApplication.Features.Attendances.Queries.Models
{
    public class GetAttendanceForMemberQuery
        : PaginationRequest, IQuery<PaginatedResult<AttendanceResponse>>
    {
    }
}
