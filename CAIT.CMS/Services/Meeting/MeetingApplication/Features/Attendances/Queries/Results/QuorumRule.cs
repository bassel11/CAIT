using MeetingCore.Enums;

namespace MeetingApplication.Features.Attendances.Queries.Results
{
    public class QuorumRule
    {
        public QuorumType Type { get; set; } = QuorumType.PercentagePlusOne;
        public decimal Threshold { get; set; } = 50m; // for Percentage-based: percent value (e.g., 50 => 50%)
        public bool UsePlusOne { get; set; } = true;  // for 50% + 1
        public int? AbsoluteCount { get; set; }        // optionally direct absolute number
    }
}
