using CommitteeCore.Enums;

namespace CommitteeCore.Entities
{
    public class CommitteeQuorumRule
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid CommitteeId { get; set; }
        public Committee Committee { get; set; }
        public QuorumType Type { get; set; } // Percentage, PercentagePlusOne, AbsoluteNumber
        public decimal? ThresholdPercent { get; set; }
        public int? AbsoluteCount { get; set; }
        public bool UsePlusOne { get; set; }
        public string? Description { get; set; }
        public DateTime EffectiveDate { get; set; } = DateTime.UtcNow;
        public DateTime? ExpiryDate { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    }
}
