using BuildingBlocks.Shared.CQRS;
using BuildingBlocks.Shared.Wrappers;

namespace MeetingApplication.Features.Meetings.Commands.Models
{
    public record CancelMeetingCommand(
    Guid Id,
    string Reason
    // Guid CancelledBy
) : ICommand<Result>;

    public record CancelMeetingRequestDto(string Reason);
}
