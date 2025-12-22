namespace TaskCore.ValueObjects
{
    public record CommitteeId
    {
        public Guid Value { get; }

        private CommitteeId(Guid value)
        {
            Value = value;
        }

        public static CommitteeId Of(Guid value)
        {
            if (value == Guid.Empty)
                throw new ArgumentException("CommitteeId cannot be empty.");

            return new CommitteeId(value);
        }

        public override string ToString() => Value.ToString();
    }
}
