using BuildingBlocks.Shared.Exceptions;
using MediatR;
using MeetingApplication.Features.Attendances.Commands.Models;
using MeetingCore.Entities;
using MeetingCore.Enums;
using MeetingCore.Repositories;

namespace MeetingApplication.Features.Attendances.Commands.Handlers
{
    public class ConfirmAttendanceCommandHandler : IRequestHandler<ConfirmAttendanceCommand, Guid>
    {
        private readonly IAttendanceRepository _repo;
        private readonly IMeetingRepository _meetings;
        //private readonly IDateTimeProvider _clock;

        public ConfirmAttendanceCommandHandler(
            IAttendanceRepository repo,
            IMeetingRepository meetings)
        {
            _repo = repo;
            _meetings = meetings;
        }

        public async Task<Guid> Handle(ConfirmAttendanceCommand req, CancellationToken ct)
        {
            var meeting = await _meetings.GetByIdAsync(req.MeetingId);

            if (meeting == null)
            {
                throw new NotFoundException(nameof(Meeting), req.MeetingId);
            }

            // check existing RSVP record for member
            var existing = await _repo.GetByMeetingAndMemberAsync(req.MeetingId, req.MemberId, ct);


            if (existing != null)
            {
                existing.RSVP = req.RSVP;
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
                RSVP = req.RSVP,
                AttendanceStatus = AttendanceStatus.None,
                Timestamp = DateTime.UtcNow
            };

            await _repo.AddAsync(rec);
            await _repo.SaveChangesAsync(ct);
            return rec.Id;
        }
    }
}
