using MediatR;
using MeetingApplication.Common;
using MeetingApplication.Features.Attendances.Queries.Results;
using MeetingApplication.Wrappers;

namespace MeetingApplication.Features.Attendances.Queries.Models
{
    public class GetAttendanceForMemberQuery
        : PaginationRequest, IRequest<PaginatedResult<GetAttendanceResponse>>
    {
    }
}
