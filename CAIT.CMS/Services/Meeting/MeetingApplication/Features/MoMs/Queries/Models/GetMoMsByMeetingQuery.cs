using MediatR;
using MeetingApplication.Common;
using MeetingApplication.Features.MoMs.Queries.Results;
using MeetingApplication.Wrappers;

namespace MeetingApplication.Features.MoMs.Queries.Models
{
    public class GetMoMsByMeetingQuery
        : PaginationRequest, IRequest<PaginatedResult<GetMinutesResponse>>
    {
        public Guid? MeetingId { get; set; }
    }
}
