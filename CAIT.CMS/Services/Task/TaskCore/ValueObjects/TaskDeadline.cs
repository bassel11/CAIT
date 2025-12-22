namespace TaskCore.ValueObjects
{
    public record TaskDeadline
    {
        public DateTime Value { get; }

        private TaskDeadline(DateTime value) => Value = value;

        public static TaskDeadline Of(DateTime value)
        {
            if (value <= DateTime.UtcNow)
                throw new ArgumentException("Task deadline must be in the future.");

            return new TaskDeadline(value);
        }

        public bool IsPassed() => DateTime.UtcNow > Value;

        public override string ToString() => Value.ToString("O");
    }
}
