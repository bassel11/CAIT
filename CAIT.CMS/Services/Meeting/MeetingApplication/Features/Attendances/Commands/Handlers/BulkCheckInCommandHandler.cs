using MediatR;
using MeetingApplication.Exceptions;
using MeetingApplication.Features.Attendances.Commands.Models;
using MeetingCore.Entities;
using MeetingCore.Enums;
using MeetingCore.Repositories;

namespace MeetingApplication.Features.Attendances.Commands.Handlers
{
    public class BulkCheckInCommandHandler : IRequestHandler<BulkCheckInCommand, Unit>
    {
        #region Fields
        private readonly IAttendanceRepository _repo;
        private readonly IMeetingRepository _meetings;
        #endregion

        #region Constructor
        public BulkCheckInCommandHandler(IAttendanceRepository repo, IMeetingRepository meetings)
        {
            _repo = repo;
            _meetings = meetings;
        }
        #endregion

        #region Actions
        public async Task<Unit> Handle(BulkCheckInCommand req, CancellationToken ct)
        {
            var meeting = await _meetings.GetByIdAsync(req.MeetingId);

            if (meeting == null)
            {
                throw new MeetingNotFoundException(nameof(Meeting), req.MeetingId);
            }

            var memberIds = req.Entries.Select(e => e.MemberId).ToList();
            var existing = await _repo.GetByMeetingAndMembersAsync(req.MeetingId, memberIds, ct);


            var now = DateTime.UtcNow;

            foreach (var e in req.Entries)
            {
                var ex = existing.FirstOrDefault(x => x.MemberId == e.MemberId);
                if (ex != null)
                {
                    ex.AttendanceStatus = e.Status;
                    ex.Timestamp = now;
                    await _repo.UpdateAsync(ex);
                }
                else
                {
                    var rec = new Attendance
                    {
                        Id = Guid.NewGuid(),
                        MeetingId = req.MeetingId,
                        MemberId = e.MemberId,
                        RSVP = RSVPStatus.Yes,
                        AttendanceStatus = e.Status,
                        Timestamp = now
                    };
                    await _repo.AddAsync(rec);
                }
            }

            await _repo.SaveChangesAsync(ct);
            return Unit.Value;
        }

        #endregion
    }
}
