using MediatR;

namespace MeetingApplication.Features.Meetings.Commands.Models
{
    public record CompleteMeetingCommand(Guid Id) : IRequest<Unit>;
}
