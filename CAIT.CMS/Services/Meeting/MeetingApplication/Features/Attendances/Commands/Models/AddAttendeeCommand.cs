using BuildingBlocks.Shared.CQRS;
using BuildingBlocks.Shared.Wrappers;
using MeetingCore.Enums.AttendanceEnums;

namespace MeetingApplication.Features.Attendances.Commands.Models
{
    public record AddAttendeeCommand(
        Guid MeetingId,
        Guid UserId,
        AttendanceRole Role,       //  الأفضل: استخدام النوع الصريح
        VotingRight VotingRight    //  الأفضل: استخدام النوع الصريح
    ) : ICommand<Result>;
}
