using MediatR;
using MeetingApplication.Common.CurrentUser;
using MeetingApplication.Common.DateTimeProvider;
using MeetingApplication.Exceptions;
using MeetingApplication.Features.MoMs.Commands.Models;
using MeetingCore.Entities;
using MeetingCore.Enums;
using MeetingCore.Repositories;

namespace MeetingApplication.Features.MoMs.Commands.Handlers
{
    public class CreateMoMCommandHandler : IRequestHandler<CreateMoMCommand, Guid>
    {
        private readonly IMoMRepository _repo;
        private readonly IMeetingRepository _meetings;
        private readonly IDateTimeProvider _clock;
        private readonly ICurrentUserService _user;

        public CreateMoMCommandHandler(IMoMRepository repo, IMeetingRepository meetings, IDateTimeProvider clock, ICurrentUserService user)
        {
            _repo = repo;
            _meetings = meetings;
            _clock = clock;
            _user = user;
        }

        public async Task<Guid> Handle(CreateMoMCommand req, CancellationToken ct)
        {
            var meeting = await _meetings.GetByIdAsync(req.MeetingId);
            if (meeting == null)
            {
                throw new MeetingNotFoundException(nameof(Meeting), req.MeetingId);
            }

            // Optional: prevent multiple MoM if one exists
            var existingMoM = await _repo.GetByMeetingIdAsync(req.MeetingId, ct);
            if (existingMoM != null)
                throw new DomainException("MoM already exists for this meeting");


            var mom = new MinutesOfMeeting
            {
                Id = Guid.NewGuid(),
                MeetingId = req.MeetingId,
                Status = MoMStatus.Draft,
                AttendanceSummary = req.AttendanceSummary,
                AgendaSummary = req.AgendaSummary,
                DecisionsSummary = req.DecisionsSummary,
                ActionItemsJson = req.ActionItemsJson,
                VersionNumber = 1,
                CreatedAt = _clock.UtcNow,
                CreatedBy = _user.UserId == Guid.Empty ? Guid.Empty : _user.UserId
            };

            await _repo.AddAsync(mom);
            await _repo.SaveChangesAsync(ct);
            return mom.Id;
        }
    }
}
