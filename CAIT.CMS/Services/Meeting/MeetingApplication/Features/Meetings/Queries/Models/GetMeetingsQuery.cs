using MediatR;
using MeetingApplication.Common;
using MeetingApplication.Features.Meetings.Queries.Results;
using MeetingApplication.Wrappers;

namespace MeetingApplication.Features.Meetings.Queries.Models
{
    public class GetMeetingsQuery
        : PaginationRequest, IRequest<PaginatedResult<GetMeetingResponse>>
    {
    }
}
