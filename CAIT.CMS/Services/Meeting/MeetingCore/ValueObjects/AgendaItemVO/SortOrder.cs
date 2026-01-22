namespace MeetingCore.ValueObjects.AgendaItemVO
{
    public record SortOrder
    {
        public int Value { get; }

        private SortOrder(int value)
        {
            if (value < 1)
                throw new DomainException("SortOrder must be greater than zero.");
            Value = value;
        }

        public static SortOrder Of(int value) => new SortOrder(value);
        public override string ToString() => Value.ToString();
    }
}
