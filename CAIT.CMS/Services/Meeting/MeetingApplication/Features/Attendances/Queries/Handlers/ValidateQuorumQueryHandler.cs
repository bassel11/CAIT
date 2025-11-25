using MediatR;
using MeetingApplication.Exceptions;
using MeetingApplication.Features.Attendances.Queries.Models;
using MeetingApplication.Features.Attendances.Queries.Results;
using MeetingApplication.Interfaces.Committee;
using MeetingCore.Entities;
using MeetingCore.Enums;
using MeetingCore.Repositories;

namespace MeetingApplication.Features.Attendances.Queries.Handlers
{
    public class ValidateQuorumQueryHandler : IRequestHandler<ValidateQuorumQuery, QuorumValidationResult>
    {
        private readonly IMeetingRepository _meetings;
        private readonly IAttendanceRepository _attendance;
        private readonly ICommitteeService _committeeService;

        public ValidateQuorumQueryHandler(
            IMeetingRepository meetings,
            IAttendanceRepository attendance,
            ICommitteeService committeeService)
        {
            _meetings = meetings;
            _attendance = attendance;
            _committeeService = committeeService;
        }

        public async Task<QuorumValidationResult> Handle(ValidateQuorumQuery req, CancellationToken ct)
        {
            var meeting = await _meetings.GetByIdAsync(req.MeetingId);
            if (meeting == null)
            {
                throw new MeetingNotFoundException(nameof(Meeting), req.MeetingId);
            }

            // Fetch data from Committee Service
            var totalMembers = await _committeeService.GetMemberCountAsync(meeting.CommitteeId, ct);

            var rule = await _committeeService.GetQuorumRuleAsync(meeting.CommitteeId, ct)
                ?? new QuorumRule
                {
                    Type = QuorumType.PercentagePlusOne,
                    Threshold = 50m,
                    UsePlusOne = true
                };

            // Count present members
            var presentCount = await _attendance.CountPresentMembersAsync(req.MeetingId, ct);

            // Calculate required quorum
            int required = 0;
            string ruleDesc = "";

            switch (rule.Type)
            {
                case QuorumType.AbsoluteNumber:
                    required = rule.AbsoluteCount ?? 0;
                    ruleDesc = $"Requires at least {required} members";
                    break;

                case QuorumType.Percentage:
                    required = (int)Math.Ceiling(totalMembers * ((rule.Threshold ?? 0m) / 100m));
                    ruleDesc = $"Requires {rule.Threshold}% of members => {required}";
                    break;

                case QuorumType.PercentagePlusOne:
                default:
                    var baseCount = (int)Math.Floor(totalMembers * ((rule.Threshold ?? 0m) / 100m));
                    required = rule.UsePlusOne ? baseCount + 1 : baseCount;
                    ruleDesc = $"Requires {rule.Threshold}% +1 => {required}";
                    break;
            }

            // Safety clamp
            required = Math.Clamp(required, 0, totalMembers);

            return new QuorumValidationResult
            {
                MeetingId = req.MeetingId,
                TotalMembers = totalMembers,
                PresentCount = presentCount,
                RequiredCount = required,
                QuorumMet = presentCount >= required,
                RuleDescription = ruleDesc,
                Note = presentCount >= required ? "Quorum satisfied" : "Quorum not met"
            };
        }
    }
}
