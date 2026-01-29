using BuildingBlocks.Shared.CQRS;
using BuildingBlocks.Shared.Wrappers;
using MeetingCore.Enums.AttendanceEnums;

namespace MeetingApplication.Features.Attendances.Commands.Models
{
    public record BulkCheckInItemDto(
        Guid UserId,
        AttendanceStatus Status,
        bool IsProxy = false,
        string? ProxyName = null
       );

    public record BulkCheckInCommand(
        Guid MeetingId,
        List<BulkCheckInItemDto> Items // ✅ استخدام DTO يحتوي على Enum
    ) : ICommand<Result>;
}
