using BuildingBlocks.Shared.CQRS;
using BuildingBlocks.Shared.Wrappers;
using MeetingApplication.Features.Attendances.Commands.Models;
using MeetingCore.Repositories;
using MeetingCore.ValueObjects.AttendanceVO;
using MeetingCore.ValueObjects.MeetingVO;
using Microsoft.Extensions.Logging;

namespace MeetingApplication.Features.Attendances.Commands.Handlers
{
    public class ConfirmAttendanceCommandHandler : ICommandHandler<ConfirmAttendanceCommand, Result>
    {
        private readonly IMeetingRepository _meetingRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly ILogger<ConfirmAttendanceCommandHandler> _logger; // Best Practice

        public ConfirmAttendanceCommandHandler(
            IMeetingRepository meetingRepository,
            ICurrentUserService currentUserService,
            ILogger<ConfirmAttendanceCommandHandler> logger)
        {
            _meetingRepository = meetingRepository;
            _currentUserService = currentUserService;
            _logger = logger;
        }

        public async Task<Result> Handle(ConfirmAttendanceCommand request, CancellationToken cancellationToken)
        {
            // 1. Load Meeting with Attendees
            // نحتاج Attendees لنتأكد أن هذا المستخدم مدعو بالفعل
            var meeting = await _meetingRepository.GetWithAttendeesAsync(MeetingId.Of(request.MeetingId), cancellationToken);

            if (meeting == null)
                return Result.Failure("Meeting not found.");

            try
            {
                var userId = _currentUserService.UserId;
                // 2. Domain Behavior
                // ✅ استخدام المعرف من التوكن (Service) هو قمة الأمان
                meeting.ConfirmRSVP(
                    UserId.Of(userId),
                    request.Status // لا حاجة للتحويل هنا
                );

                // 3. Save
                await _meetingRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("User {UserId} confirmed RSVP {Status} for meeting {MeetingId}", userId, request.Status, request.MeetingId);
                return Result.Success("RSVP confirmed successfully.");
            }
            catch (DomainException ex)
            {
                return Result.Failure(ex.Message);
            }
        }
    }
}
