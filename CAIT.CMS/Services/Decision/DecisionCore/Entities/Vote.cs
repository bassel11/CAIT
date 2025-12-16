using DecisionCore.Abstractions;
using DecisionCore.Enums;
using DecisionCore.ValueObjects;

namespace DecisionCore.Entities
{
    public class Vote : Entity<VoteId>
    {
        public DecisionId DecisionId { get; private set; } = default!;
        public Decision Decision { get; private set; } = default!; // ✅ مهم
        public Guid MemberId { get; private set; } = default!;
        public VoteType Type { get; private set; } = default!;
        public DateTime VotedAt { get; private set; } = default!;

        private Vote() { } // For EF Core

        internal Vote(DecisionId decisionId, Guid memberId, VoteType type)
        {
            Id = VoteId.Of(Guid.NewGuid());
            DecisionId = decisionId;
            MemberId = memberId;
            Type = type;
            VotedAt = DateTime.UtcNow;
        }

    }

}
