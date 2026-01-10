namespace TaskCore.ValueObjects
{
    public record DecisionId
    {
        public Guid Value { get; }

        private DecisionId(Guid value)
        {
            Value = value;
        }

        public static DecisionId Of(Guid value)
        {
            if (value == Guid.Empty)
                throw new DomainException("DecisionId cannot be empty.");

            return new DecisionId(value);
        }

        public override string ToString() => Value.ToString();
    }
}
