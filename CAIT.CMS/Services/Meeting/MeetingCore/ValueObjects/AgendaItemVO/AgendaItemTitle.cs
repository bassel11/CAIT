namespace MeetingCore.ValueObjects.AgendaItemVO
{
    public record AgendaItemTitle
    {
        public string Value { get; }

        private AgendaItemTitle(string value)
        {
            Value = value;
        }

        public static AgendaItemTitle Of(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new DomainException("Title cannot be empty.");

            // إذا أردت، يمكن إضافة تحقق لطول محدد
            // if (value.Length > 100) throw new ArgumentException("Title too long.");

            return new AgendaItemTitle(value);
        }

        public override string ToString() => Value;
    }
}
