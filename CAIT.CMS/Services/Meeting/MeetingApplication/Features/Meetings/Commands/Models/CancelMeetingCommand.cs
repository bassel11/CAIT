using MediatR;

namespace MeetingApplication.Features.Meetings.Commands.Models
{
    public record CancelMeetingCommand(Guid Id, string? Reason = null) : IRequest<Unit>;
}
