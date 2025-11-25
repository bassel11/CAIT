using MediatR;
using MeetingApplication.Features.Meetings.Queries.Results;
using MeetingApplication.Responses;

namespace MeetingApplication.Features.Meetings.Queries.Models
{
    public class GetMeetingsByCommitteeIdQuery
        : IRequest<Response<IEnumerable<GetMeetingResponse>>>
    {
        public Guid CommitteeId { get; set; }

        public GetMeetingsByCommitteeIdQuery(Guid committeeId)
        {
            CommitteeId = committeeId;
        }
    }
}
