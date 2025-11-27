using MediatR;
using MeetingApplication.Features.MoMs.Queries.Results;

namespace MeetingApplication.Features.MoMs.Queries.Models
{
    public class GetMoMsForMeetingQuery : IRequest<List<GetMinutesResponse>>
    {
        public Guid MeetingId { get; set; }
    }
}
