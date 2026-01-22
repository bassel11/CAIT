using BuildingBlocks.Shared.CQRS;
using BuildingBlocks.Shared.Wrappers;

namespace MeetingApplication.Features.Meetings.Commands.Models
{
    public record UpdateMeetingCommand(
        Guid Id,
        string Title,
        string? Description,
        // Location Info
        int LocationType,
        string? LocationRoom,
        string? LocationAddress,
        string? LocationOnlineUrl
    // Guid UpdatedBy
    ) : ICommand<Result<Guid>>;
}
