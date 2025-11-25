using CommitteeCore.Enums;

namespace CommitteeApplication.Features.CommitteeQuorumRules.Queries.Results
{
    public class CommitteeQuorumRuleResponse
    {
        public Guid Id { get; set; }
        public Guid CommitteeId { get; set; }
        public QuorumType Type { get; set; }
        public decimal? ThresholdPercent { get; set; }
        public int? AbsoluteCount { get; set; }
        public bool UsePlusOne { get; set; }
        public string? Description { get; set; }
        public DateTime EffectiveDate { get; set; }
        public DateTime? ExpiryDate { get; set; }

    }
}
