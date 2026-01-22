using BuildingBlocks.Shared.CQRS;
using BuildingBlocks.Shared.Wrappers;

namespace MeetingApplication.Features.Attendances.Commands.Models
{
    public record RemoveAttendeeCommand(Guid MeetingId, Guid UserId)
        : ICommand<Result>;
}
