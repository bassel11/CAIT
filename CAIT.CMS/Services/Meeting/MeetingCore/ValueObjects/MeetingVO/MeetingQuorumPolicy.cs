namespace MeetingCore.ValueObjects.MeetingVO
{
    public sealed record MeetingQuorumPolicy
    {
        public QuorumType Type { get; }
        public decimal? ThresholdPercent { get; }
        public int? AbsoluteCount { get; }
        public bool UsePlusOne { get; }

        // Constructor خاص لـ EF Core
        private MeetingQuorumPolicy() { }

        private MeetingQuorumPolicy(QuorumType type, decimal? percent, int? count, bool plusOne)
        {
            Type = type;
            ThresholdPercent = percent;
            AbsoluteCount = count;
            UsePlusOne = plusOne;
        }

        public static MeetingQuorumPolicy Create(
            QuorumType type,
            decimal? percent,
            int? count,
            bool usePlusOne)
        {
            if (type == QuorumType.Percentage && (percent == null || percent <= 0))
                throw new DomainException("ThresholdPercent is required for Percentage quorum.");

            if (type == QuorumType.AbsoluteNumber && (count == null || count <= 0))
                throw new DomainException("AbsoluteCount is required for AbsoluteNumber quorum.");

            return new MeetingQuorumPolicy(type, percent, count, usePlusOne);
        }

        // دالة مساعدة للعرض
        public string GetDescription() => Type switch
        {
            QuorumType.AbsoluteNumber => $"Min {AbsoluteCount} members",
            QuorumType.Percentage => $"{ThresholdPercent}% of members",
            QuorumType.PercentagePlusOne => $"50% + 1 (Majority)",
            _ => "Unknown Rule"
        };
    }
}
