namespace DecisionCore.ValueObjects
{
    public sealed record VoteId
    {
        public Guid Value { get; }

        private VoteId(Guid value) => Value = value;

        //public static VoteId New() => new VoteId(Guid.NewGuid());

        public static VoteId Of(Guid value)
        {
            if (value == Guid.Empty)
                throw new DomainException("VoteId cannot be empty.");
            return new VoteId(value);
        }

        //public override string ToString() => Value.ToString();
    }
}
