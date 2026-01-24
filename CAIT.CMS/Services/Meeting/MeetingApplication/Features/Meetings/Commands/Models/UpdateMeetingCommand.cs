using BuildingBlocks.Shared.CQRS;
using BuildingBlocks.Shared.Wrappers;
using MeetingCore.Enums.MeetingEnums;

namespace MeetingApplication.Features.Meetings.Commands.Models
{
    public record UpdateMeetingCommand(
        Guid Id,
        string Title,
        string? Description,
        // Location Info
        LocationType LocationType, // 1=Physical, 2=Online, 3=Hybrid (نستقبل رقم أو نص)
        string? LocationRoom,
        string? LocationAddress,
        string? LocationOnlineUrl
    // Guid UpdatedBy
    ) : ICommand<Result<Guid>>;
}
