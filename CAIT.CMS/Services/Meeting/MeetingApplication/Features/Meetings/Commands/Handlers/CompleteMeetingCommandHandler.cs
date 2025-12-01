using AutoMapper;
using MediatR;
using MeetingApplication.Common.CurrentUser;
using MeetingApplication.Exceptions;
using MeetingApplication.Features.Meetings.Commands.Models;
using MeetingApplication.Interfaces;
using MeetingCore.Entities;
using MeetingCore.Enums;
using MeetingCore.Repositories;
using Microsoft.Extensions.Logging;

namespace MeetingApplication.Features.Meetings.Commands.Handlers
{
    public class CompleteMeetingCommandHandler : IRequestHandler<CompleteMeetingCommand, Unit>
    {
        #region Fields
        private readonly IMeetingRepository _meetingRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<CancelMeetingCommandHandler> _logger;
        private readonly ICurrentUserService _currentUserService;
        private readonly IUnitOfWork _unitOfWork;
        #endregion

        #region Constructor
        public CompleteMeetingCommandHandler(IMeetingRepository meetingRepository
                                             , IMapper mapper
                                             , ILogger<CancelMeetingCommandHandler> logger
                                             , ICurrentUserService currentUserService
                                             , IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _meetingRepository = meetingRepository;
            _mapper = mapper;
            _currentUserService = currentUserService;
            _unitOfWork = unitOfWork;
        }
        #endregion

        #region Actions
        public async Task<Unit> Handle(CompleteMeetingCommand request, CancellationToken cancellationToken)
        {
            var meetingCompleted = await _meetingRepository.GetByIdAsync(request.Id);
            if (meetingCompleted == null) throw new MeetingNotFoundException(nameof(Meeting), request.Id);

            if (meetingCompleted.Status == MeetingStatus.Completed)
                throw new DomainException("Meeting already completed");

            meetingCompleted.Status = MeetingStatus.Completed;
            meetingCompleted.UpdatedAt = DateTime.UtcNow;
            meetingCompleted.UpdatedBy = _currentUserService.UserId;

            await _meetingRepository.UpdateAsync(meetingCompleted);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
        #endregion


    }
}
