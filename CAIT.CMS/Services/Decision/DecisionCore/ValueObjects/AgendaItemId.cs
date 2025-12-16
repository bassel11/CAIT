namespace DecisionCore.ValueObjects
{
    public record AgendaItemId
    {
        public Guid Value { get; }

        private AgendaItemId(Guid value)
        {
            Value = value;
        }

        public static AgendaItemId Of(Guid value)
        {
            if (value == Guid.Empty)
                throw new ArgumentException("AgendaItemId cannot be empty.");

            return new AgendaItemId(value);
        }

        public override string ToString() => Value.ToString();
    }
}
