using BuildingBlocks.Shared.CQRS;
using BuildingBlocks.Shared.Wrappers;
using MeetingApplication.Features.MoMAttendances.Commands.Models;
using MeetingCore.Repositories;
using MeetingCore.ValueObjects.MeetingVO;
using MeetingCore.ValueObjects.MoMAttendanceVO;

namespace MeetingApplication.Features.MoMAttendances.Commands.Handlers
{
    public class CorrectMoMAttendanceCommandHandler : ICommandHandler<CorrectMoMAttendanceCommand, Result>
    {
        private readonly IMinutesRepository _repo;
        private readonly ICurrentUserService _user;

        public CorrectMoMAttendanceCommandHandler(IMinutesRepository repo, ICurrentUserService user)
        {
            _repo = repo;
            _user = user;
        }

        public async Task<Result> Handle(CorrectMoMAttendanceCommand req, CancellationToken ct)
        {
            // جلب المحضر مع قائمة الحضور (Full Graph أو Include AttendanceSnapshot)
            var mom = await _repo.GetFullGraphByMeetingIdAsync(MeetingId.Of(req.MeetingId), ct);

            if (mom == null) return Result.Failure("Minutes not found.");

            try
            {
                // استدعاء دالة الدومين
                mom.CorrectAttendance(
                    MoMAttendanceId.Of(req.AttendanceRowId),
                    req.NewStatus,
                    req.Notes
                );

                // حفظ التغييرات
                await _repo.UnitOfWork.SaveChangesAsync(ct);
                return Result.Success("Attendance corrected successfully.");
            }
            catch (DomainException ex)
            {
                return Result.Failure(ex.Message);
            }
        }
    }
}
