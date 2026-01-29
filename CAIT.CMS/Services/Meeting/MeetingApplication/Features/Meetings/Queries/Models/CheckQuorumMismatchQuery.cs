using BuildingBlocks.Shared.CQRS;
using BuildingBlocks.Shared.Wrappers;
using MeetingApplication.Features.Meetings.Queries.Results;

namespace MeetingApplication.Features.Meetings.Queries.Models
{
    public record CheckQuorumMismatchQuery(Guid MeetingId) : IQuery<Result<QuorumMismatchResult>>;
}
