namespace TaskCore.ValueObjects
{
    public record MeetingId
    {
        public Guid Value { get; }

        private MeetingId(Guid value)
        {
            Value = value;
        }

        public static MeetingId Of(Guid value)
        {
            if (value == Guid.Empty)
                throw new ArgumentException("MeetingId cannot be empty.");

            return new MeetingId(value);
        }

        public override string ToString() => Value.ToString();
    }
}
