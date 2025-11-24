using MediatR;

namespace MeetingApplication.Features.Meetings.Commands.Models
{
    public class CompleteMeetingCommand : IRequest<Unit>
    {
        public Guid Id { get; set; }
    }
}
