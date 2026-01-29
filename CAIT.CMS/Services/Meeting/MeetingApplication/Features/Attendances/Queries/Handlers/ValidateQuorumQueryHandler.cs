using BuildingBlocks.Shared.CQRS;
using BuildingBlocks.Shared.Wrappers;
using MeetingApplication.Features.Attendances.Queries.Models;
using MeetingApplication.Features.Attendances.Queries.Results;
using MeetingCore.Enums;
using MeetingCore.Enums.AttendanceEnums;
using MeetingCore.Repositories;
using MeetingCore.ValueObjects.MeetingVO;

namespace MeetingApplication.Features.Attendances.Queries.Handlers
{
    public class ValidateQuorumQueryHandler : IQueryHandler<ValidateQuorumQuery, Result<QuorumValidationResult>>
    {
        private readonly IMeetingRepository _repository;

        public ValidateQuorumQueryHandler(IMeetingRepository repository)
        {
            _repository = repository;
        }

        public async Task<Result<QuorumValidationResult>> Handle(ValidateQuorumQuery request, CancellationToken ct)
        {
            // 1. جلب الاجتماع مع الحضور من المستودع
            // نحتاج الحضور لحساب الأعداد، ونحتاج الاجتماع لقراءة الـ QuorumPolicy
            var meeting = await _repository.GetWithAttendeesAsync(MeetingId.Of(request.MeetingId), ct);

            if (meeting == null)
                return Result<QuorumValidationResult>.Failure("Meeting not found.");

            // 2. التحقق من النصاب باستخدام منطق الدومين (Snapshot Logic)
            bool isMet = meeting.IsQuorumMet();

            // 3. حساب الأرقام للعرض في الواجهة (ViewModel Logic)
            // عدد الأعضاء الذين يحق لهم التصويت في هذا الاجتماع
            var totalVotingMembers = meeting.Attendances.Count(a => a.VotingRight == VotingRight.Voting);

            // عدد الحضور الفعليين (بمن فيهم النواب) الذين يحق لهم التصويت
            var presentVotingMembers = meeting.Attendances.Count(a => a.CountsForQuorum());

            // حساب العدد المطلوب تحقيقه (نعيد تطبيق نفس معادلة الدومين للعرض)
            int requiredCount = CalculateRequired(meeting.QuorumPolicy, totalVotingMembers);

            // 4. إرجاع النتيجة
            return Result<QuorumValidationResult>.Success(new QuorumValidationResult
            {
                MeetingId = request.MeetingId,
                TotalVotingMembers = totalVotingMembers,
                PresentVotingMembers = presentVotingMembers,
                RequiredCount = requiredCount,
                QuorumMet = isMet,
                // نستخدم دالة الوصف الموجودة في الـ Value Object
                RuleDescription = meeting.QuorumPolicy.GetDescription(),
                StatusMessage = isMet
                    ? "Quorum is satisfied. You can proceed to official decisions."
                    : $"Quorum not met. Need {Math.Max(0, requiredCount - presentVotingMembers)} more voting member(s)."
            });
        }

        // منطق حساب العدد المطلوب للعرض (يمكن نقله لداخل MeetingQuorumPolicy لاحقاً)
        private int CalculateRequired(MeetingQuorumPolicy policy, int total)
        {
            if (total == 0) return 0;

            return policy.Type switch
            {
                QuorumType.AbsoluteNumber => policy.AbsoluteCount ?? 0,

                QuorumType.Percentage =>
                    (int)Math.Ceiling(total * ((policy.ThresholdPercent ?? 0) / 100m)),

                QuorumType.PercentagePlusOne =>
                    (total / 2) + 1,

                _ => 0
            };
        }
    }
}