using MediatR;
using MeetingApplication.Features.Meetings.Queries.Results;
using MeetingApplication.Responses;

namespace MeetingApplication.Features.Meetings.Queries.Models
{
    public class GetMeetingByIdQuery : IRequest<Response<GetMeetingResponse>>
    {
        public Guid Id { get; set; }

        public GetMeetingByIdQuery(Guid id)
        {
            Id = id;
        }
    }
}
