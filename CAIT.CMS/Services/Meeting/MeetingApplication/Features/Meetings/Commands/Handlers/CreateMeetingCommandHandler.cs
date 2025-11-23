using AutoMapper;
using MediatR;
using MeetingApplication.Features.Meetings.Commands.Models;
using MeetingApplication.Features.Meetings.Commands.Results;
using MeetingCore.Repositories;

namespace MeetingApplication.Features.Meetings.Commands.Handlers
{
    public class CreateMeetingCommandHandler : IRequestHandler<CreateMeetingCommand, MeetingResponse>
    {
        #region Fields

        private readonly IMeetingRepository _meetingRepository;
        private readonly IMapper _mapper;

        #endregion

        #region Constructor

        public CreateMeetingCommandHandler(IMeetingRepository meetingRepository, IMapper mapper)
        {
            _meetingRepository = meetingRepository;
            _mapper = mapper;
        }

        #endregion

        #region Actions
        public Task<MeetingResponse> Handle(CreateMeetingCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
