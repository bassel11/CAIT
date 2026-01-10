using MediatR;

namespace MeetingApplication.Features.Attendances.Commands.Models
{
    public record RemoveAttendanceCommand(Guid Id) : IRequest<Unit>;
}
