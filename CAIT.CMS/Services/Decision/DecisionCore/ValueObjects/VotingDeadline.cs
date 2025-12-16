namespace DecisionCore.ValueObjects
{
    public sealed record VotingDeadline
    {
        public DateTime Value { get; }

        private VotingDeadline(DateTime value) => Value = value;

        public static VotingDeadline Of(DateTime value)
        {
            if (value <= DateTime.UtcNow)
                throw new ArgumentException("Voting deadline must be in the future.");

            return new VotingDeadline(value);
        }

        public bool IsPassed() => DateTime.UtcNow > Value;

        public override string ToString() => Value.ToString("O");
    }
}
