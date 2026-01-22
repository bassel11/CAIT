using BuildingBlocks.Shared.CQRS;
using BuildingBlocks.Shared.Wrappers;
using MeetingCore.Enums.AttendanceEnums;

namespace MeetingApplication.Features.Attendances.Commands.Models
{
    public record ConfirmAttendanceCommand(
        Guid MeetingId,
        RSVPStatus Status // ✅ أفضل ممارسة: استخدام الـ Enum مباشرة
    ) : ICommand<Result>;
}
