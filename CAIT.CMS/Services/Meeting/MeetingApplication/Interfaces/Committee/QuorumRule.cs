using MeetingCore.Enums;

namespace MeetingApplication.Interfaces.Committee
{
    //public class QuorumRule
    // {
    //public QuorumType Type { get; set; } = QuorumType.PercentagePlusOne;
    //public decimal? Threshold { get; set; } = 50m; // for Percentage-based: percent value (e.g., 50 => 50%)
    //public bool UsePlusOne { get; set; } = true;  // for 50% + 1
    //public int? AbsoluteCount { get; set; }        // optionally direct absolute number
    // }

    // DTOs يجب أن تكون هنا أو في ملفات Shared

    public class QuorumRule // أو record
    {
        public QuorumType Type { get; set; }
        public decimal? ThresholdPercent { get; set; }
        public bool UsePlusOne { get; set; }
        public int? AbsoluteCount { get; set; }
    }
}
