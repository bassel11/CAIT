using MediatR;
using MeetingApplication.Features.MoMs.Commands.Models;
using MeetingApplication.Integrations;
using MeetingCore.Entities;
using MeetingCore.Enums;
using MeetingCore.Events;
using MeetingCore.Repositories;

namespace MeetingApplication.Features.MoMs.Commands.Handlers
{
    public class RejectMoMCommandHandler : IRequestHandler<RejectMoMCommand, Guid>
    {
        private readonly IMoMRepository _repo;
        private readonly ICurrentUserService _user;
        private readonly IDateTimeProvider _clock;
        private readonly IEventBus _eventBus;

        public RejectMoMCommandHandler(IMoMRepository repo, ICurrentUserService user, IDateTimeProvider clock, IEventBus eventBus)
        {
            _repo = repo;
            _user = user;
            _clock = clock;
            _eventBus = eventBus;
        }

        public async Task<Guid> Handle(RejectMoMCommand req, CancellationToken ct)
        {
            var mom = await _repo.GetByIdAsync(req.Id);
            if (mom == null)
            {
                throw new NotFoundException(nameof(MinutesOfMeeting), req.Id);
            }

            if (mom.Status == MoMStatus.Approved)
                throw new DomainException("Cannot reject an approved MoM");

            if (mom.Status != MoMStatus.PendingApproval)
                throw new DomainException("Only pending MoMs can be rejected.");

            mom.Status = MoMStatus.Rejected;
            mom.ApprovedAt = _clock.UtcNow;
            mom.ApprovedBy = _user.UserId;

            var note = new MinutesVersion
            {
                Id = Guid.NewGuid(),
                MoMId = mom.Id,
                Content = $"[Rejection Reason by {_user.UserId} at {_clock.UtcNow}] {req.Reason}",
                VersionNumber = mom.VersionNumber + 1,
                CreatedAt = _clock.UtcNow,
                CreatedBy = _user.UserId
            };

            mom.Versions.Add(note);
            mom.VersionNumber += 1;

            await _repo.AddVersionAsync(note, ct);
            await _repo.UpdateAsync(mom);
            await _repo.SaveChangesAsync(ct);

            await _eventBus.PublishAsync(new MoMDraftUpdatedEvent(mom.Id, mom.MeetingId, note.CreatedBy, note.CreatedAt), ct);

            return mom.Id;
        }
    }
}
