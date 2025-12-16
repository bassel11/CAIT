namespace DecisionCore.ValueObjects
{
    public record DecisionTitle
    {
        public string Value { get; }

        private DecisionTitle(string value)
        {
            Value = value;
        }

        public static DecisionTitle Of(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Title cannot be empty.");

            // إذا أردت، يمكن إضافة تحقق لطول محدد
            // if (value.Length > 100) throw new ArgumentException("Title too long.");

            return new DecisionTitle(value);
        }

        public override string ToString() => Value;
    }
}
