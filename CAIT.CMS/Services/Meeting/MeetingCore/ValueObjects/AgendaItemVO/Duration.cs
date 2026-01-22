namespace MeetingCore.ValueObjects.AgendaItemVO
{
    public sealed record Duration
    {
        public TimeSpan Value { get; }

        private Duration(TimeSpan value)
        {
            if (value <= TimeSpan.Zero)
                throw new DomainException("Duration must be greater than zero.");

            Value = value;
        }

        public static Duration FromMinutes(double minutes)
        {
            return new Duration(TimeSpan.FromMinutes(minutes));
        }

        public static Duration Of(TimeSpan value) => new Duration(value);

        public override string ToString() => Value.ToString();
    }
}
