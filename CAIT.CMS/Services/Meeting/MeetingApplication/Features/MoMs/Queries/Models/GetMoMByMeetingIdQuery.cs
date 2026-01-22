using BuildingBlocks.Shared.CQRS;
using BuildingBlocks.Shared.Wrappers;
using MeetingApplication.Features.MoMs.Queries.Results;

namespace MeetingApplication.Features.MoMs.Queries.Models
{
    public class GetMoMByMeetingIdQuery : IQuery<Result<MoMResponse>>
    {
        public Guid MeetingId { get; set; }
        public GetMoMByMeetingIdQuery(Guid meetingId) => MeetingId = meetingId;
    }
}
