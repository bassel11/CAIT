using BuildingBlocks.Shared.CQRS;
using BuildingBlocks.Shared.Wrappers;
using MeetingCore.Enums.AttendanceEnums;

namespace MeetingApplication.Features.MoMAttendances.Commands.Models
{
    public record CorrectMoMAttendanceCommand(
    Guid MeetingId,
    Guid AttendanceRowId,
    AttendanceStatus NewStatus,
    string? Notes) : ICommand<Result>;
}
