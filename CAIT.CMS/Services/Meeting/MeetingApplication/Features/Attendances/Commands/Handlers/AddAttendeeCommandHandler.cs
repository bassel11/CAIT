using BuildingBlocks.Shared.CQRS;
using BuildingBlocks.Shared.Wrappers;
using MeetingApplication.Features.Attendances.Commands.Models;
using MeetingCore.Repositories;
using MeetingCore.ValueObjects.AttendanceVO;
using MeetingCore.ValueObjects.MeetingVO;

namespace MeetingApplication.Features.Attendances.Commands.Handlers
{
    public class AddAttendeeCommandHandler : ICommandHandler<AddAttendeeCommand, Result>
    {
        private readonly IMeetingRepository _meetingRepository;

        public AddAttendeeCommandHandler(IMeetingRepository meetingRepository)
        {
            _meetingRepository = meetingRepository;
        }

        public async Task<Result> Handle(AddAttendeeCommand request, CancellationToken cancellationToken)
        {
            // 1. Load Meeting with Attendances
            // يجب تحميل الاجتماع مع الحضور للتحقق من عدم تكرار العضو
            var meeting = await _meetingRepository.GetWithAttendeesAsync(MeetingId.Of(request.MeetingId), cancellationToken);

            if (meeting == null)
                return Result.Failure("Meeting not found.");

            try
            {
                // 2. Domain Behavior
                meeting.AddAttendee(
                    UserId.Of(request.UserId),
                    request.Role,         // تمرير مباشر بدون (AttendanceRole)
                    request.VotingRight   // تمرير مباشر بدون (VotingRight)
                );

                // 3. Save
                await _meetingRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
                return Result.Success("Attendee added successfully.");
            }
            catch (DomainException ex)
            {
                return Result.Failure(ex.Message);
            }
        }
    }
}
