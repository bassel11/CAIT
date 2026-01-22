namespace MeetingCore.ValueObjects.MinutesVersionVO
{
    public record MinutesVersionId
    {
        public Guid Value { get; }
        private MinutesVersionId(Guid value)
        {
            Value = value;
        }
        public static MinutesVersionId Of(Guid value)
        {
            if (value == Guid.Empty)
                throw new DomainException("MinutesVersionId cannot be empty.");

            return new MinutesVersionId(value);
        }

        //public static MinutesVersionId New() => new MinutesVersionId(Guid.NewGuid());

        //public override string ToString() => Value.ToString();
    }

}
