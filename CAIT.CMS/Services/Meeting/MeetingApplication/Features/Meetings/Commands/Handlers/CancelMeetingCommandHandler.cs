using AutoMapper;
using BuildingBlocks.Shared.Exceptions;
using MediatR;
using MeetingApplication.Features.Meetings.Commands.Models;
using MeetingApplication.Interfaces;
using MeetingCore.Entities;
using MeetingCore.Enums;
using MeetingCore.Repositories;
using Microsoft.Extensions.Logging;

namespace MeetingApplication.Features.Meetings.Commands.Handlers
{
    public class CancelMeetingCommandHandler : IRequestHandler<CancelMeetingCommand, Unit>
    {
        #region Fields
        private readonly IMeetingRepository _meetingRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<CancelMeetingCommandHandler> _logger;
        private readonly ICurrentUserService _currentUserService;
        private readonly IUnitOfWork _unitOfWork;
        #endregion

        #region Constructor
        public CancelMeetingCommandHandler(IMeetingRepository meetingRepository
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

        public async Task<Unit> Handle(CancelMeetingCommand request, CancellationToken cancellationToken)
        {
            var meeting = await _meetingRepository.GetByIdAsync(request.Id);

            if (meeting == null)
                throw new NotFoundException(nameof(Meeting), request.Id);

            if (meeting.Status == MeetingStatus.Cancelled)
                throw new DomainException("Meeting already cancelled");

            meeting.Status = MeetingStatus.Cancelled;
            meeting.UpdatedAt = DateTime.UtcNow;
            meeting.UpdatedBy = _currentUserService.UserId;

            var log = new MeetingIntegrationLog
            {
                Id = Guid.NewGuid(),
                MeetingId = meeting.Id,
                IntegrationType = IntegrationType.OutlookCancel,
                Success = true,
                ErrorMessage = $"Cancelled: {request.Reason}",
                CreatedAt = DateTime.UtcNow
            };

            meeting.IntegrationLogs.Add(log);

            // سجل التعديل في DbContext فقط
            await _meetingRepository.UpdateAsync(meeting);

            // التعديلات على meeting + log ستحفظ معًا
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Unit.Value;



        }

        #endregion



    }
}
