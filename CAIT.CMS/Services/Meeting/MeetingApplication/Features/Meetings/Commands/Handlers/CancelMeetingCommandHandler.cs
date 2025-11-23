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
    public class CancelMeetingCommandHandler : IRequestHandler<CancelMeetingCommand, Unit>
    {
        #region Fields
        private readonly IMeetingRepository _meetingRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<CancelMeetingCommandHandler> _logger;
        private readonly ICurrentUserService _currentUserService;
        #endregion

        #region Constructor
        public CancelMeetingCommandHandler(IMeetingRepository meetingRepository
                                             , IMapper mapper
                                             , ILogger<CancelMeetingCommandHandler> logger
                                             , ICurrentUserService currentUserService)
        {
            _logger = logger;
            _meetingRepository = meetingRepository;
            _mapper = mapper;
            _currentUserService = currentUserService;
        }

        #endregion

        #region Actions

        public async Task<Unit> Handle(CancelMeetingCommand request, CancellationToken cancellationToken)
        {
            var canceledMeting = await _meetingRepository.GetByIdAsync(request.Id);

            if (canceledMeting == null)
            {
                throw new MeetingNotFoundException(nameof(Meeting), request.Id);
            }

            if (canceledMeting.Status == MeetingStatus.Cancelled)
                throw new DomainException("Meeting already cancelled");

            canceledMeting.Status = MeetingStatus.Cancelled;
            canceledMeting.UpdatedAt = DateTime.UtcNow;
            canceledMeting.UpdatedBy = _currentUserService.UserId;

            // Optionally store cancellation reason in IntegrationLogs or a new column
            var log = new MeetingIntegrationLog
            {
                Id = Guid.NewGuid(),
                MeetingId = canceledMeting.Id,
                IntegrationType = IntegrationType.OutlookCancel,
                Success = true,
                ExternalId = null,
                ErrorMessage = $"Cancelled: {request.Reason}",
                CreatedAt = DateTime.UtcNow
            };

            // attach
            canceledMeting.IntegrationLogs.Add(log);

            await _meetingRepository.UpdateAsync(canceledMeting);
            //await _meetingRepository.SaveChangesAsync(cancellationToken);

            return Unit.Value;



        }

        #endregion



    }
}
