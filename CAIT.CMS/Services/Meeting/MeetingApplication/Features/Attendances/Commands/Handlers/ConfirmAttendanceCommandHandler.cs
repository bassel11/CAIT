using BuildingBlocks.Shared.CQRS;
using BuildingBlocks.Shared.Wrappers;
using MeetingApplication.Features.Attendances.Commands.Models;
using MeetingCore.Repositories;
using MeetingCore.ValueObjects.AttendanceVO;
using MeetingCore.ValueObjects.MeetingVO;

namespace MeetingApplication.Features.Attendances.Commands.Handlers
{
    public class ConfirmAttendanceCommandHandler : ICommandHandler<ConfirmAttendanceCommand, Result>
    {
        private readonly IMeetingRepository _meetingRepository;
        private readonly ICurrentUserService _currentUserService;

        public ConfirmAttendanceCommandHandler(
            IMeetingRepository meetingRepository,
            ICurrentUserService currentUserService)
        {
            _meetingRepository = meetingRepository;
            _currentUserService = currentUserService;
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
                // 2. Domain Behavior
                // ✅ استخدام المعرف من التوكن (Service) هو قمة الأمان
                meeting.ConfirmRSVP(
                    UserId.Of(_currentUserService.UserId),
                    request.Status // لا حاجة للتحويل هنا
                );

                // 3. Save
                await _meetingRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
                return Result.Success("RSVP confirmed successfully.");
            }
            catch (DomainException ex)
            {
                return Result.Failure(ex.Message);
            }
        }
    }
}
