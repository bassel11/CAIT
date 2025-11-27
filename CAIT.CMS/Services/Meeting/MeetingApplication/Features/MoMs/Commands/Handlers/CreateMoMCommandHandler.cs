using MediatR;
using MeetingApplication.Common.CurrentUser;
using MeetingApplication.Common.DateTimeProvider;
using MeetingApplication.Exceptions;
using MeetingApplication.Features.MoMs.Commands.Models;
using MeetingApplication.Interfaces.Integrations;
using MeetingCore.Entities;
using MeetingCore.Enums;
using MeetingCore.Events;
using MeetingCore.Repositories;

namespace MeetingApplication.Features.MoMs.Commands.Handlers
{
    public class CreateMoMCommandHandler : IRequestHandler<CreateMoMCommand, Guid>
    {
        private readonly IMoMRepository _repo;
        private readonly IMeetingRepository _meetings;
        private readonly IDateTimeProvider _clock;
        private readonly ICurrentUserService _user;
        private readonly IEventBus _bus;

        public CreateMoMCommandHandler(IMoMRepository repo
                                     , IMeetingRepository meetings
                                     , IDateTimeProvider clock
                                     , ICurrentUserService user
                                     , IEventBus bus)
        {
            _repo = repo;
            _meetings = meetings;
            _clock = clock;
            _user = user;
            _bus = bus;
        }

        public async Task<Guid> Handle(CreateMoMCommand req, CancellationToken ct)
        {
            var meeting = await _meetings.GetByIdAsync(req.MeetingId);
            if (meeting == null)
            {
                throw new MeetingNotFoundException(nameof(Meeting), req.MeetingId);
            }

            // Business rule: cannot create MoM before meeting ended
            if (meeting.EndDate > _clock.UtcNow)
                throw new DomainException("Cannot create MoM before meeting ends.");

            // Optional: prevent multiple MoM if one exists
            var existingMoM = await _repo.GetByMeetingIdAsync(req.MeetingId, ct);
            if (existingMoM != null)
                throw new DomainException("MoM already exists for this meeting");

            int nextVersion = existingMoM?.VersionNumber + 1 ?? 1;


            var mom = new MinutesOfMeeting
            {
                Id = Guid.NewGuid(),
                MeetingId = req.MeetingId,
                Status = MoMStatus.Draft,
                AttendanceSummary = req.AttendanceSummary,
                AgendaSummary = req.AgendaSummary,
                DecisionsSummary = req.DecisionsSummary,
                ActionItemsJson = req.ActionItemsJson,
                VersionNumber = nextVersion,
                CreatedAt = _clock.UtcNow,
                CreatedBy = _user.UserId == Guid.Empty ? Guid.Empty : _user.UserId
            };

            // initial content as first version
            if (!string.IsNullOrWhiteSpace(req.InitialContent))
            {
                var ver = new MinutesVersion
                {
                    Id = Guid.NewGuid(),
                    MoMId = mom.Id,
                    Content = req.InitialContent!,
                    VersionNumber = nextVersion,
                    CreatedAt = _clock.UtcNow,
                    CreatedBy = mom.CreatedBy
                };
                mom.Versions.Add(ver);
            }

            await _repo.AddAsync(mom);
            await _repo.SaveChangesAsync(ct);

            // publish domain event
            await _bus.PublishAsync(new MoMDraftCreatedEvent(mom.Id, req.MeetingId, mom.CreatedBy, mom.CreatedAt), ct);

            return mom.Id;
        }
    }
}
