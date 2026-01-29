using MeetingCore.Enums;

namespace MeetingApplication.Features.Meetings.Queries.Results
{
    public record QuorumMismatchResult
    {
        public bool HasMismatch { get; init; }

        // البيانات الحالية في الاجتماع (Snapshot)
        public string CurrentPolicyDescription { get; init; } = default!;

        // البيانات الجديدة في اللجنة (Live)
        public string NewPolicyDescription { get; init; } = default!;

        // التفاصيل التقنية (اختياري، في حال أردت عرض مقارنة دقيقة)
        public QuorumType? NewType { get; init; }
        public decimal? NewThreshold { get; init; }
    }
}
