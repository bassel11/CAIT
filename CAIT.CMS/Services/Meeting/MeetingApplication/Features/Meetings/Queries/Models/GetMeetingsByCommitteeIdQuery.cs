using BuildingBlocks.Shared.CQRS;
using BuildingBlocks.Shared.Wrappers;
using MeetingApplication.Features.Meetings.Queries.Results;

namespace MeetingApplication.Features.Meetings.Queries.Models
{
    public record GetMeetingsByCommitteeIdQuery(Guid CommitteeId)
        : IQuery<Result<List<GetMeetingResponse>>>;
}
