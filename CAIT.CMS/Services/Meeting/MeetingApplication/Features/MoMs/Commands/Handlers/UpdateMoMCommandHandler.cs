using MediatR;
using MeetingApplication.Features.MoMs.Commands.Models;
using MeetingApplication.Integrations;
using MeetingCore.Entities;
using MeetingCore.Enums;
using MeetingCore.Repositories;

namespace MeetingApplication.Features.MoMs.Commands.Handlers
{
    public class UpdateMoMCommandHandler : IRequestHandler<UpdateMoMCommand, Guid>
    {
        private readonly IMoMRepository _repo;
        private readonly IDateTimeProvider _clock;
        private readonly ICurrentUserService _user;
        private readonly IEventBus _bus;

        public UpdateMoMCommandHandler(IMoMRepository repo
                                     , IDateTimeProvider clock
                                     , ICurrentUserService user
                                     , IEventBus bus)
        {
            _repo = repo;
            _clock = clock;
            _user = user;
            _bus = bus;
        }

        public async Task<Guid> Handle(UpdateMoMCommand req, CancellationToken ct)
        {
            var mom = await _repo.GetByIdAsync(req.MoMId);
            if (mom == null)
            {
                throw new NotFoundException(nameof(MinutesOfMeeting), req.MoMId);
            }

            if (mom.Status != MoMStatus.Draft && mom.Status != MoMStatus.PendingApproval)
                throw new DomainException("Cannot update an approved MoM");

            // append new version
            int newVersion = mom.VersionNumber + 1;
            var version = new MinutesVersion
            {
                Id = Guid.NewGuid(),
                MoMId = mom.Id,
                Content = req.Content,
                VersionNumber = newVersion,
                CreatedAt = _clock.UtcNow,
                CreatedBy = _user.UserId == Guid.Empty ? Guid.Empty : _user.UserId
            };
            mom.Versions.Add(version);
            mom.VersionNumber = newVersion;

            mom.AttendanceSummary = req.AttendanceSummary ?? mom.AttendanceSummary;
            mom.AgendaSummary = req.AgendaSummary ?? mom.AgendaSummary;
            mom.DecisionsSummary = req.DecisionsSummary ?? mom.DecisionsSummary;
            mom.ActionItemsJson = req.ActionItemsJson ?? mom.ActionItemsJson;
            mom.CreatedAt = _clock.UtcNow; // optional: track last update
            mom.CreatedBy = _user.UserId == Guid.Empty ? Guid.Empty : _user.UserId;

            await _repo.AddVersionAsync(version, ct);
            await _repo.UpdateAsync(mom);
            await _repo.SaveChangesAsync(ct);

            //await _bus.PublishAsync(new MoMDraftUpdatedEvent(mom.Id, mom.MeetingId, version.CreatedBy, version.CreatedAt), ct);

            return mom.Id;
        }
    }
}
