using BuildingBlocks.Shared.CQRS;
using MeetingApplication.Data;
using MeetingApplication.Features.Attendances.Queries.Models;
using MeetingApplication.Features.Attendances.Queries.Results;
using MeetingApplication.Interfaces.Committee;
using MeetingCore.Entities;
using MeetingCore.Enums;
using MeetingCore.Enums.AttendanceEnums;
using MeetingCore.ValueObjects.MeetingVO;

namespace MeetingApplication.Features.Attendances.Queries.Handlers
{
    public class ValidateQuorumQueryHandler : IQueryHandler<ValidateQuorumQuery, QuorumValidationResult>
    {
        private readonly IMeetingDbContext _dbContext;
        private readonly ICommitteeService _committeeService;

        public ValidateQuorumQueryHandler(
            IMeetingDbContext dbContext,
            ICommitteeService committeeService)
        {
            _dbContext = dbContext;
            _committeeService = committeeService;
        }

        public async Task<QuorumValidationResult> Handle(ValidateQuorumQuery req, CancellationToken ct)
        {
            var meetingId = MeetingId.Of(req.MeetingId);

            // -----------------------------------------------------------
            // 1. الخطوة الأولى: جلب بيانات الاجتماع وعدد الحضور الفعليين
            // -----------------------------------------------------------
            // نستخدم Projection لجلب ما نحتاجه فقط (CommitteeId + Count)
            var meetingData = await _dbContext.Meetings
                .AsNoTracking()
                .Where(m => m.Id == meetingId)
                .Select(m => new
                {
                    m.CommitteeId,
                    // العد الذكي: (حاضر أو أونلاين) + (يملك حق التصويت)
                    PresentVotersCount = m.Attendances.Count(a =>
                        (a.AttendanceStatus == AttendanceStatus.Present || a.AttendanceStatus == AttendanceStatus.Remote)
                        && a.VotingRight == VotingRight.Voting
                    )
                })
                .FirstOrDefaultAsync(ct);

            if (meetingData == null)
            {
                throw new NotFoundException(nameof(Meeting), req.MeetingId);
            }

            // -----------------------------------------------------------
            // 2. الخطوة الثانية: جلب بيانات اللجنة (قواعد النصاب)
            // -----------------------------------------------------------
            // نحصل على العدد الكلي للأعضاء في اللجنة
            var totalMembers = await _committeeService.GetMemberCountAsync(meetingData.CommitteeId.Value, ct);

            // نحصل على قاعدة النصاب، وإذا لم تكن محددة نستخدم قاعدة افتراضية (50% + 1)
            var rule = await _committeeService.GetQuorumRuleAsync(meetingData.CommitteeId.Value, ct)
                ?? new QuorumRule
                {
                    Type = QuorumType.PercentagePlusOne,
                    Threshold = 50m,
                    UsePlusOne = true
                };

            // -----------------------------------------------------------
            // 3. الخطوة الثالثة: تطبيق منطق الحساب (Business Logic)
            // -----------------------------------------------------------
            int requiredCount = CalculateRequiredCount(rule, totalMembers);

            // التحقق النهائي
            bool isMet = meetingData.PresentVotersCount >= requiredCount;

            // -----------------------------------------------------------
            // 4. بناء النتيجة
            // -----------------------------------------------------------
            return new QuorumValidationResult
            {
                MeetingId = req.MeetingId,
                TotalMembers = totalMembers,
                PresentCount = meetingData.PresentVotersCount,
                RequiredCount = requiredCount,
                QuorumMet = isMet,
                RuleDescription = GetRuleDescription(rule),
                StatusMessage = isMet
                    ? "Quorum is satisfied. You can proceed."
                    : $"Quorum not met. Need {requiredCount - meetingData.PresentVotersCount} more member(s)."
            };
        }

        // ============================================================
        // Helper Methods (للحفاظ على نظافة الـ Handle)
        // ============================================================

        private int CalculateRequiredCount(QuorumRule rule, int total)
        {
            if (total == 0) return 0; // حماية من الأخطاء المنطقية

            decimal threshold = rule.Threshold ?? 0;

            switch (rule.Type)
            {
                case QuorumType.AbsoluteNumber:
                    // عدد ثابت (مثلاً: يجب حضور 5 أشخاص مهما كان العدد الكلي)
                    // نستخدم Min لضمان عدم طلب عدد أكبر من الأعضاء الموجودين
                    return Math.Min(rule.AbsoluteCount ?? 0, total);

                case QuorumType.Percentage:
                    // نسبة مئوية (مثلاً: 60% من الأعضاء)
                    // Ceiling لضمان جبر الكسر للأعلى (مثلاً 5.1 تصبح 6)
                    return (int)Math.Ceiling(total * (threshold / 100m));

                case QuorumType.PercentagePlusOne:
                default:
                    // النسبة الشائعة (50% + 1)
                    // Floor ثم إضافة 1
                    var baseCount = (int)Math.Floor(total * (threshold / 100m));
                    return rule.UsePlusOne ? baseCount + 1 : baseCount;
            }
        }

        private string GetRuleDescription(QuorumRule rule)
        {
            return rule.Type switch
            {
                QuorumType.AbsoluteNumber => $"Fixed Number ({rule.AbsoluteCount})",
                QuorumType.Percentage => $"Percentage ({rule.Threshold}%)",
                QuorumType.PercentagePlusOne => $"Majority ({rule.Threshold}% + 1)",
                _ => "Standard Rule"
            };
        }
    }
}
