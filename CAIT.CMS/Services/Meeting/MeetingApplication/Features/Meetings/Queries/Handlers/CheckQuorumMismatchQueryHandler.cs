using BuildingBlocks.Shared.CQRS;
using BuildingBlocks.Shared.Wrappers;
using MeetingApplication.Features.Meetings.Queries.Models;
using MeetingApplication.Features.Meetings.Queries.Results;
using MeetingApplication.Interfaces.Committee;
using MeetingCore.Enums;
using MeetingCore.Repositories;
using MeetingCore.ValueObjects.MeetingVO;

namespace MeetingApplication.Features.Meetings.Queries.Handlers
{
    public class CheckQuorumMismatchQueryHandler : IQueryHandler<CheckQuorumMismatchQuery, Result<QuorumMismatchResult>>
    {
        private readonly IMeetingRepository _meetingRepository;
        private readonly ICommitteeService _committeeService; // ✅ نحتاجه هنا للمقارنة

        public CheckQuorumMismatchQueryHandler(
            IMeetingRepository meetingRepository,
            ICommitteeService committeeService)
        {
            _meetingRepository = meetingRepository;
            _committeeService = committeeService;
        }

        public async Task<Result<QuorumMismatchResult>> Handle(CheckQuorumMismatchQuery request, CancellationToken ct)
        {
            // 1. جلب الاجتماع (لقراءة الـ Snapshot الحالي)
            var meeting = await _meetingRepository.GetByIdAsync(MeetingId.Of(request.MeetingId), ct);
            if (meeting == null) return Result<QuorumMismatchResult>.Failure("Meeting not found.");

            // 2. جلب القاعدة الحالية من اللجنة (Live Data)
            var liveRuleDto = await _committeeService.GetQuorumRuleAsync(meeting.CommitteeId.Value, ct);

            // إذا لم نجد قاعدة في اللجنة، نفترض الافتراضي أو نعتبره عدم تطابق حسب البيزنس
            // هنا سنفترض الافتراضي للمقارنة
            var livePolicy = liveRuleDto != null
                ? MeetingQuorumPolicy.Create(liveRuleDto.Type, liveRuleDto.ThresholdPercent, liveRuleDto.AbsoluteCount, liveRuleDto.UsePlusOne)
                : MeetingQuorumPolicy.Create(QuorumType.PercentagePlusOne, null, null, true);

            // 3. المقارنة (Logic of Truth)
            // نستخدم Equals لأن MeetingQuorumPolicy هو (record) يدعم مقارنة القيم تلقائياً
            bool isDifferent = !meeting.QuorumPolicy.Equals(livePolicy);

            // 4. إعداد النتيجة
            var result = new QuorumMismatchResult
            {
                HasMismatch = isDifferent,
                CurrentPolicyDescription = meeting.QuorumPolicy.GetDescription(),
                NewPolicyDescription = livePolicy.GetDescription(),

                // بيانات إضافية في حال رغبت الواجهة بعرض التفاصيل
                NewType = livePolicy.Type,
                NewThreshold = livePolicy.ThresholdPercent
            };

            return Result<QuorumMismatchResult>.Success(result);
        }
    }
}
