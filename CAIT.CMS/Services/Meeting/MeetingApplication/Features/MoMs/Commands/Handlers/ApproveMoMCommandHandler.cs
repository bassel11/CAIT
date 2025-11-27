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
    public class ApproveMoMCommandHandler : IRequestHandler<ApproveMoMCommand, Unit>
    {
        private readonly IMoMRepository _repo;
        private readonly ICurrentUserService _user;
        private readonly IDateTimeProvider _clock;

        public ApproveMoMCommandHandler(IMoMRepository repo, ICurrentUserService user, IDateTimeProvider clock)
        {
            _repo = repo;
            _user = user;
            _clock = clock;
        }

        public async Task<Unit> Handle(ApproveMoMCommand req, CancellationToken ct)
        {
            var mom = await _repo.GetByIdAsync(req.MoMId);
            if (mom == null)
            {
                throw new MoMNotFoundException(nameof(MinutesOfMeeting), req.MoMId);
            }

            if (mom.Status == MoMStatus.Approved)
                throw new DomainException("MoM already approved");

            mom.Status = MoMStatus.Approved;
            mom.ApprovedAt = _clock.UtcNow;
            mom.ApprovedBy = _user.UserId;

            await _repo.UpdateAsync(mom);
            await _repo.SaveChangesAsync(ct);

            return Unit.Value;
        }
    }
}
