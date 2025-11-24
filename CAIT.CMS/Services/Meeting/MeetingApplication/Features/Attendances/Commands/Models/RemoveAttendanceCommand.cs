using MediatR;

namespace MeetingApplication.Features.Attendances.Commands.Models
{
    public class RemoveAttendanceCommand : IRequest<Unit>
    {
        public Guid Id { get; set; }
    }
}
