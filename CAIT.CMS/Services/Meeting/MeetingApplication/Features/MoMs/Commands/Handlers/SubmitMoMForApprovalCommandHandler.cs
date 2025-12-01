using MediatR;
using MeetingApplication.Common.CurrentUser;
using MeetingApplication.Common.DateTimeProvider;
using MeetingApplication.Exceptions;
using MeetingApplication.Features.MoMs.Commands.Models;
using MeetingApplication.Integrations;
using MeetingApplication.Interfaces.Committee;
using MeetingCore.Entities;
using MeetingCore.Enums;
using MeetingCore.Events;
using MeetingCore.Repositories;

namespace MeetingApplication.Features.MoMs.Commands.Handlers
{
    public class SubmitMoMForApprovalCommandHandler
        : IRequestHandler<SubmitMoMForApprovalCommand, Unit>
    {
        private readonly IMoMRepository _momRepo;
        private readonly IMeetingRepository _meetRepo;
        private readonly IAttendanceRepository _attendanceRepo;
        private readonly ICommitteeService _committee;
        private readonly IEventBus _bus;
        private readonly IDateTimeProvider _clock;
        private readonly ICurrentUserService _user;

        public SubmitMoMForApprovalCommandHandler(IMoMRepository momRepo, IMeetingRepository meetRepo, IAttendanceRepository attendanceRepo, ICommitteeService committee, IEventBus bus, IDateTimeProvider clock, ICurrentUserService user)
        {
            _momRepo = momRepo;
            _meetRepo = meetRepo;
            _attendanceRepo = attendanceRepo;
            _committee = committee;
            _bus = bus;
            _clock = clock;
            _user = user;
        }

        public async Task<Unit> Handle(SubmitMoMForApprovalCommand req, CancellationToken ct)
        {
            var mom = await _momRepo.GetByIdAsync(req.MoMId);
            if (mom == null)
            {
                throw new MoMNotFoundException(nameof(MinutesOfMeeting), req.MoMId);
            }
            var meeting = await _meetRepo.GetByIdAsync(mom.MeetingId);
            if (meeting == null)
            {
                throw new MeetingNotFoundException(nameof(Meeting), mom.MeetingId);
            }

            // Business rule: ensure quorum met before submit (configurable; can be relaxed)
            var total = await _committee.GetMemberCountAsync(meeting.CommitteeId, ct);
            var present = await _attendanceRepo.CountPresentMembersAsync(meeting.Id, ct);

            // fetch quorum rule and calculate required if needed - use committee service for more complex logic
            var rule = await _committee.GetQuorumRuleAsync(meeting.CommitteeId, ct);
            int required =
                (rule?.Type == QuorumType.AbsoluteNumber)
                ? (rule.AbsoluteCount ?? 0)
                : (int)Math.Ceiling(total * ((rule?.Threshold ?? 50m) / 100m)) + (rule?.UsePlusOne == true ? 1 : 0);

            if (present < Math.Clamp(required, 0, total))
                throw new DomainException("Quorum not met — cannot submit MoM for approval.");

            mom.Status = MoMStatus.PendingApproval;
            //mom.UpdatedAt = _clock.UtcNow;
            await _momRepo.UpdateAsync(mom);
            await _momRepo.SaveChangesAsync(ct);

            await _bus.PublishAsync(new MoMSubmittedForApprovalEvent(mom.Id, mom.MeetingId, _user.UserId == Guid.Empty ? Guid.Empty : _user.UserId, _clock.UtcNow), ct);

            // Queue notification to Chairman via NotificationService or EventBus downstream (not shown)
            return Unit.Value;
        }
    }
}
