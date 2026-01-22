namespace MeetingCore.ValueObjects.AgendaItemVO
{
    public record PresenterId
    {
        public Guid Value { get; }

        private PresenterId(Guid value)
        {
            if (value == Guid.Empty)
                throw new DomainException("UserId cannot be empty.");
            Value = value;
        }

        public static PresenterId Of(Guid value) => new PresenterId(value);
        public override string ToString() => Value.ToString();
    }
}
