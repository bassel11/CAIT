using MediatR;
using MeetingApplication.Features.Attendances.Commands.Models;
using MeetingCore.Entities;
using MeetingCore.Enums;
using MeetingCore.Repositories;

namespace MeetingApplication.Features.Attendances.Commands.Handlers
{
    public class CheckInAttendanceCommandHandler : IRequestHandler<CheckInAttendanceCommand, Guid>
    {
        private readonly IAttendanceRepository _repo;
        private readonly IMeetingRepository _meetings;

        public CheckInAttendanceCommandHandler(IAttendanceRepository repo, IMeetingRepository meetings)
        {
            _repo = repo;
            _meetings = meetings;
        }

        public async Task<Guid> Handle(CheckInAttendanceCommand req, CancellationToken ct)
        {
            var meeting = await _meetings.GetByIdAsync(req.MeetingId);

            if (meeting == null)
            {
                throw new NotFoundException(nameof(Meeting), req.MeetingId);
            }

            // optionally: only allow check-in during [StartDate - X .. EndDate + Y] window. (Business rule)
            // find existing record
            var existing = await _repo.GetByMeetingAndMemberAsync(req.MeetingId, req.MemberId, ct);

            if (existing != null)
            {
                existing.AttendanceStatus = req.AttendanceStatus;
                existing.Timestamp = DateTime.UtcNow;
                await _repo.UpdateAsync(existing);
                await _repo.SaveChangesAsync(ct);
                return existing.Id;
            }

            var rec = new Attendance
            {
                Id = Guid.NewGuid(),
                MeetingId = req.MeetingId,
                MemberId = req.MemberId,
                RSVP = RSVPStatus.Accepted, // default to yes if checking-in
                AttendanceStatus = req.AttendanceStatus,
                Timestamp = DateTime.UtcNow
            };

            await _repo.AddAsync(rec);
            await _repo.SaveChangesAsync(ct);
            return rec.Id;
        }
    }
}
