using BuildingBlocks.Shared.CQRS;
using BuildingBlocks.Shared.Wrappers;
using MeetingApplication.Features.Attendances.Commands.Models;
using MeetingCore.Repositories;
using MeetingCore.ValueObjects.AttendanceVO;
using MeetingCore.ValueObjects.MeetingVO;

namespace MeetingApplication.Features.Attendances.Commands.Handlers
{
    public class CheckInAttendeeCommandHandler : ICommandHandler<CheckInAttendeeCommand, Result>
    {
        private readonly IMeetingRepository _meetingRepository;

        public CheckInAttendeeCommandHandler(IMeetingRepository meetingRepository)
        {
            _meetingRepository = meetingRepository;
        }

        public async Task<Result> Handle(CheckInAttendeeCommand request, CancellationToken cancellationToken)
        {
            var meeting = await _meetingRepository.GetWithAttendeesAsync(MeetingId.Of(request.MeetingId), cancellationToken);
            if (meeting == null) return Result.Failure("Meeting not found.");

            try
            {
                meeting.CheckInAttendee(UserId.Of(request.UserId), request.IsRemote);
                await _meetingRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
                return Result.Success("Checked in successfully.");
            }
            catch (DomainException ex)
            {
                return Result.Failure(ex.Message);
            }
        }
    }
}
