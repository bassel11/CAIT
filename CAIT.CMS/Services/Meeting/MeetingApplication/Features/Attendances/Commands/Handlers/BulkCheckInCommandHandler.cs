using MediatR;
using MeetingApplication.Features.Attendances.Commands.Models;
using MeetingCore.Entities;
using MeetingCore.Enums;
using MeetingCore.Repositories;

namespace MeetingApplication.Features.Attendances.Commands.Handlers
{
    public class BulkCheckInCommandHandler : IRequestHandler<BulkCheckInCommand, Unit>
    {
        private readonly IAttendanceRepository _repo;

        public BulkCheckInCommandHandler(IAttendanceRepository repo)
        {
            _repo = repo;
        }

        public async Task<Unit> Handle(BulkCheckInCommand req, CancellationToken ct)
        {
            // 1️⃣ تحقق من وجود الاجتماع
            bool meetingExists = await _repo.ExistsAsync(req.MeetingId, ct);
            if (!meetingExists)
                throw new NotFoundException(nameof(Meeting), req.MeetingId);

            var memberIds = req.Entries.Select(e => e.MemberId).ToList();

            // 2️⃣ جلب الحضور الحالي فقط للأعضاء المطلوبين
            var existingAttendances = await _repo.GetByMeetingAndMembersAsync(req.MeetingId, memberIds, ct);

            var now = DateTime.UtcNow;
            var toAdd = new List<Attendance>();
            var toUpdate = new List<Attendance>();

            foreach (var entry in req.Entries)
            {
                var exist = existingAttendances.FirstOrDefault(x => x.MemberId == entry.MemberId);
                if (exist != null)
                {
                    exist.AttendanceStatus = entry.Status;
                    exist.Timestamp = now;
                    toUpdate.Add(exist);
                }
                else
                {
                    toAdd.Add(new Attendance
                    {
                        Id = Guid.NewGuid(),
                        MeetingId = req.MeetingId,
                        MemberId = entry.MemberId,
                        AttendanceStatus = entry.Status,
                        RSVP = RSVPStatus.Accepted,
                        Timestamp = now
                    });
                }
            }

            // 3️⃣ نفذ Bulk Update و Add
            if (toUpdate.Any())
                _repo.BulkUpdate(toUpdate);

            if (toAdd.Any())
                await _repo.BulkAddAsync(toAdd, ct);

            // 4️⃣ حفظ التغييرات مرة واحدة
            await _repo.SaveChangesAsync(ct);

            return Unit.Value;
        }
    }
}
