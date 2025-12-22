namespace TaskCore.ValueObjects
{
    public record TaskTitle
    {
        public string Value { get; }

        private TaskTitle(string value)
        {
            Value = value;
        }

        public static TaskTitle Of(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Title cannot be empty.");

            // إذا أردت، يمكن إضافة تحقق لطول محدد
            // if (value.Length > 100) throw new ArgumentException("Title too long.");

            return new TaskTitle(value);
        }

        public override string ToString() => Value;
    }
}
