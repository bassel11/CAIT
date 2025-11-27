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
    public class UpdateMoMCommandHandler : IRequestHandler<UpdateMoMCommand, Guid>
    {
        private readonly IMoMRepository _repo;
        private readonly IDateTimeProvider _clock;
        private readonly ICurrentUserService _user;

        public UpdateMoMCommandHandler(IMoMRepository repo, IDateTimeProvider clock, ICurrentUserService user)
        {
            _repo = repo;
            _clock = clock;
            _user = user;
        }

        public async Task<Guid> Handle(UpdateMoMCommand req, CancellationToken ct)
        {
            var mom = await _repo.GetByIdAsync(req.MoMId);
            if (mom == null)
            {
                throw new MoMNotFoundException(nameof(MinutesOfMeeting), req.MoMId);
            }

            if (mom.Status != MoMStatus.Draft && mom.Status != MoMStatus.PendingApproval)
                throw new DomainException("Cannot update an approved MoM");

            mom.AttendanceSummary = req.AttendanceSummary;
            mom.AgendaSummary = req.AgendaSummary;
            mom.DecisionsSummary = req.DecisionsSummary;
            mom.ActionItemsJson = req.ActionItemsJson;
            mom.VersionNumber += 1;
            mom.CreatedAt = _clock.UtcNow; // optional: track last update
            mom.CreatedBy = _user.UserId == Guid.Empty ? Guid.Empty : _user.UserId;

            await _repo.UpdateAsync(mom);
            await _repo.SaveChangesAsync(ct);

            return mom.Id;
        }
    }
}
