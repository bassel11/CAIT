namespace TaskCore.ValueObjects
{
    public record TaskDescription
    {
        public string Value { get; }

        private TaskDescription(string value)
        {
            Value = value;
        }

        public static TaskDescription Of(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new DomainException("Description cannot be empty.");

            // إذا أردت، يمكن إضافة تحقق لطول محدد
            // if (value.Length > 100) throw new ArgumentException("Title too long.");

            return new TaskDescription(value);
        }

        public override string ToString() => Value;
    }
}
