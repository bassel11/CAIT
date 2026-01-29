using BuildingBlocks.Shared.CQRS;
using BuildingBlocks.Shared.Wrappers;

namespace MeetingApplication.Features.Meetings.Commands.Models
{
    public record RefreshMeetingQuorumCommand(Guid MeetingId)
        : ICommand<Result<string>>;
}
