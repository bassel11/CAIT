using BuildingBlocks.Shared.CQRS;
using BuildingBlocks.Shared.Wrappers;
using MeetingCore.Enums.MeetingEnums;

namespace MeetingApplication.Features.Meetings.Commands.Models
{
    public record CreateMeetingCommand(
        Guid CommitteeId,
        string Title,
        string? Description,

        DateTime StartDate,
        DateTime EndDate,
        string TimeZone, // e.g., "Syria Standard Time"

        // بيانات الموقع (Location)
        LocationType LocationType, // 1=Physical, 2=Online, 3=Hybrid (نستقبل رقم أو نص)
        string? LocationRoom,
        string? LocationAddress,
        string? LocationOnlineUrl,

        // بيانات التكرار (Recurrence)
        bool IsRecurring,
        RecurrenceType? RecurrenceType, // 1=Daily, 2=Weekly...
        string? RecurrenceRule
    // Guid CreatedBy
    ) : ICommand<Result<Guid>>;
}
