using AutoMapper;
using MediatR;
using MeetingApplication.Exceptions;
using MeetingApplication.Features.Meetings.Commands.Models;
using MeetingApplication.Features.Meetings.Commands.Results;
using MeetingCore.Entities;
using MeetingCore.Repositories;
using Microsoft.Extensions.Logging;

namespace MeetingApplication.Features.Meetings.Commands.Handlers
{
    public class UpdateMeetingCommandHandler : IRequestHandler<UpdateMeetingCommand, UpdateMeetingResponse>
    {
        #region Fields
        private readonly IMeetingRepository _meetingRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<UpdateMeetingCommandHandler> _logger;
        #endregion

        #region Constructor
        public UpdateMeetingCommandHandler(IMeetingRepository meetingRepository
                                         , IMapper mapper
                                         , ILogger<UpdateMeetingCommandHandler> logger)
        {
            _meetingRepository = meetingRepository;
            _mapper = mapper;
            _logger = logger;
        }
        #endregion

        #region Actions

        public async Task<UpdateMeetingResponse> Handle(UpdateMeetingCommand request, CancellationToken cancellationToken)
        {
            var meetingToUpdate = await _meetingRepository.GetByIdAsync(request.Id);
            if (meetingToUpdate == null)
            {
                throw new MeetingNotFoundException(nameof(Meeting), request.Id);
            }
            _mapper.Map(request, meetingToUpdate, typeof(UpdateMeetingCommand), typeof(Meeting));
            await _meetingRepository.UpdateAsync(meetingToUpdate);

            _logger.LogInformation($"Meeting {meetingToUpdate.Id} is successfully updated");

            // Map updated entity to response
            var response = _mapper.Map<UpdateMeetingResponse>(meetingToUpdate);
            return response;
        }

        #endregion
    }
}
