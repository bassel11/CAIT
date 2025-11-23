using MediatR;

namespace MeetingApplication.Features.Meetings.Commands.Models
{
    public class CancelMeetingCommand : IRequest<Unit>
    {
        public Guid Id { get; set; }
        public string Reason { get; set; }
    }
}
