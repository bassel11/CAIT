using MediatR;
using MeetingApplication.Features.Meetings.Commands.Models;
using MeetingApplication.Features.Meetings.Commands.Results;

namespace MeetingApplication.Features.Meetings.Commands.Handlers
{
    public class CreateMeetingCommandHandler : IRequestHandler<CreateMeetingCommand, MeetingResponse>
    {
        public Task<MeetingResponse> Handle(CreateMeetingCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
