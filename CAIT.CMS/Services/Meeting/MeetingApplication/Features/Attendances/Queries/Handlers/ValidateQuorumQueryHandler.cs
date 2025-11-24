using MediatR;
using MeetingApplication.Features.Attendances.Queries.Models;
using MeetingApplication.Features.Attendances.Queries.Results;
using MeetingCore.Repositories;

namespace MeetingApplication.Features.Attendances.Queries.Handlers
{
    public class ValidateQuorumQueryHandler : IRequestHandler<ValidateQuorumQuery, QuorumValidationResult>
    {
        private readonly IMeetingRepository _meetings;
        private readonly IAttendanceRepository _attendance;
        //private readonly ICommitteeService _committeeService;

        public ValidateQuorumQueryHandler(IMeetingRepository meetings, IAttendanceRepository attendance)
        {
            _meetings = meetings;
            _attendance = attendance;
            // _committeeService = committeeService;
        }

        public async Task<QuorumValidationResult> Handle(ValidateQuorumQuery req, CancellationToken ct)
        {
            //var meeting = await _meetings.GetByIdAsync(req.MeetingId);
            //if (meeting == null)
            //{
            //    throw new MeetingNotFoundException(nameof(Meeting), req.MeetingId);
            //}

            //// get member count from committee service
            //var totalMembers = await _committeeService.GetMemberCountAsync(meeting.CommitteeId, ct);
            //var rule = await _committeeService.GetQuorumRuleAsync(meeting.CommitteeId, ct)
            //           ?? new QuorumRule { Type = QuorumType.PercentagePlusOne, Threshold = 50m, UsePlusOne = true };

            //// Count present members: define present as AttendanceStatus.Present OR Remote (configurable)
            //var presentCount = await _attendance.Query()
            //    .Where(a => a.MeetingId == req.MeetingId &&
            //                (a.AttendanceStatus == AttendanceStatus.Present || a.AttendanceStatus == AttendanceStatus.Remote))
            //    .Select(a => a.MemberId)
            //    .Distinct()
            //    .CountAsync(ct);

            //int required = 0;
            //string ruleDesc = "";
            //switch (rule.Type)
            //{
            //    case QuorumType.AbsoluteNumber:
            //        required = rule.AbsoluteCount ?? 0;
            //        ruleDesc = $"Requires at least {required} members";
            //        break;

            //    case QuorumType.Percentage:
            //        required = (int)Math.Ceiling(totalMembers * (rule.Threshold / 100m));
            //        ruleDesc = $"Requires {rule.Threshold}% of members => {required}";
            //        break;

            //    case QuorumType.PercentagePlusOne:
            //    default:
            //        var baseCount = (int)Math.Floor(totalMembers * (rule.Threshold / 100m));
            //        required = rule.UsePlusOne ? baseCount + 1 : baseCount;
            //        ruleDesc = $"Requires {rule.Threshold}% +1 => {required}";
            //        break;
            //}

            //// safety: clamp required between 0 and totalMembers
            //required = Math.Clamp(required, 0, totalMembers);

            //var result = new QuorumValidationResult
            //{
            //    MeetingId = req.MeetingId,
            //    TotalMembers = totalMembers,
            //    PresentCount = presentCount,
            //    RequiredCount = required,
            //    QuorumMet = presentCount >= required,
            //    RuleDescription = ruleDesc,
            //    Note = presentCount >= required ? "Quorum satisfied" : "Quorum not met"
            //};

            //return result;
            return null;
        }
    }
}
