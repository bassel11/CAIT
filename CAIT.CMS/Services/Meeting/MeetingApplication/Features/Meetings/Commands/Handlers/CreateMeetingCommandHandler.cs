using AutoMapper;
using MediatR;
using MeetingApplication.Features.Meetings.Commands.Models;
using MeetingApplication.Features.Meetings.Commands.Results;
using MeetingCore.Entities;
using MeetingCore.Repositories;
using Microsoft.Extensions.Logging;

namespace MeetingApplication.Features.Meetings.Commands.Handlers
{
    public class CreateMeetingCommandHandler : IRequestHandler<CreateMeetingCommand, CreateMeetingResponse>
    {
        #region Fields

        private readonly IMeetingRepository _meetingRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<CreateMeetingCommandHandler> _logger;

        #endregion

        #region Constructor

        public CreateMeetingCommandHandler(IMeetingRepository meetingRepository
                                         , IMapper mapper
                                         , ILogger<CreateMeetingCommandHandler> logger)
        {
            _meetingRepository = meetingRepository;
            _mapper = mapper;
            _logger = logger;
        }

        #endregion

        #region Actions
        public async Task<CreateMeetingResponse> Handle(CreateMeetingCommand request, CancellationToken cancellationToken)
        {
            if (request.EndDate <= request.StartDate)
                throw new ArgumentException("EndDate must be greater than StartDate");

            // تحويل الـ Command إلى كيان (Entity)
            var meetingEntity = _mapper.Map<Meeting>(request);

            // حفظ الكيان في قاعدة البيانات باستخدام Repository
            var createdMeeting = await _meetingRepository.AddAsync(meetingEntity);

            _logger.LogInformation($"Meeting with Id {createdMeeting.Id} Successfully Created");

            // تحويل الكيان المحفوظ إلى Response
            var meetingResponse = _mapper.Map<CreateMeetingResponse>(createdMeeting);
            return meetingResponse;
        }
        #endregion
    }
}
