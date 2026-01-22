using BuildingBlocks.Shared.CQRS;
using BuildingBlocks.Shared.Wrappers;

namespace MeetingApplication.Features.Meetings.Commands.Models
{
    public record RescheduleMeetingCommand(
    Guid Id,
    DateTime NewStartDate,
    DateTime NewEndDate
    // Guid ModifiedBy
) : ICommand<Result>;
}
