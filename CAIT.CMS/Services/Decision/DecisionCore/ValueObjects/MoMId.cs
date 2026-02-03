namespace DecisionCore.ValueObjects
{
    public record MoMId
    {
        public Guid Value { get; }

        private MoMId(Guid value)
        {
            Value = value;
        }

        public static MoMId Of(Guid value)
        {
            if (value == Guid.Empty)
                throw new DomainException("MoMId cannot be empty.");

            return new MoMId(value);
        }

        public override string ToString() => Value.ToString();
    }
}
