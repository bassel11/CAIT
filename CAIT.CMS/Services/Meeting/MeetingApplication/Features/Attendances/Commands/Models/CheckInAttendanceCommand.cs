using BuildingBlocks.Shared.CQRS;
using BuildingBlocks.Shared.Wrappers;

namespace MeetingApplication.Features.Attendances.Commands.Models
{
    public record CheckInAttendeeCommand(Guid MeetingId, Guid UserId, bool IsRemote)
        : ICommand<Result>;
}
