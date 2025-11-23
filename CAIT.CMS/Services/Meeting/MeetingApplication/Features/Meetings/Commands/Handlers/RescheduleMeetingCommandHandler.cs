using AutoMapper;
using MediatR;
using MeetingApplication.Common.CurrentUser;
using MeetingApplication.Exceptions;
using MeetingApplication.Features.Meetings.Commands.Models;
using MeetingCore.Entities;
using MeetingCore.Enums;
using MeetingCore.Repositories;
using Microsoft.Extensions.Logging;

namespace MeetingApplication.Features.Meetings.Commands.Handlers
{
    public class RescheduleMeetingCommandHandler : IRequestHandler<RescheduleMeetingCommand, Unit>
    {
        #region Fields
        private readonly IMeetingRepository _meetingRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<CreateMeetingCommandHandler> _logger;
        private readonly ICurrentUserService _currentUserService;
        #endregion

        #region Constructor
        public RescheduleMeetingCommandHandler(IMeetingRepository meetingRepository
                                             , IMapper mapper
                                             , ILogger<CreateMeetingCommandHandler> logger
                                             , ICurrentUserService currentUserService)
        {
            _logger = logger;
            _meetingRepository = meetingRepository;
            _mapper = mapper;
            _currentUserService = currentUserService;
        }
        #endregion

        #region Actions
        public async Task<Unit> Handle(RescheduleMeetingCommand request, CancellationToken cancellationToken)
        {
            var rescheduledMeeting = await _meetingRepository.GetByIdAsync(request.Id);
            if (rescheduledMeeting == null)
            {
                throw new MeetingNotFoundException(nameof(Meeting), request.Id);
            }

            if (rescheduledMeeting.Status == MeetingStatus.Cancelled || rescheduledMeeting.Status == MeetingStatus.Completed)
                throw new DomainException("Cannot reschedule a cancelled or completed meeting");

            // Update
            rescheduledMeeting.StartDate = request.StartDate.ToUniversalTime();
            rescheduledMeeting.EndDate = request.EndDate.ToUniversalTime();
            rescheduledMeeting.Status = MeetingStatus.Rescheduled;
            rescheduledMeeting.UpdatedAt = DateTime.UtcNow;
            rescheduledMeeting.UpdatedBy = _currentUserService.UserId;

            await _meetingRepository.UpdateAsync(rescheduledMeeting);

            // Optionally: create integration/outbox notification for reschedule
            return Unit.Value;

        }

        #endregion
    }
}
