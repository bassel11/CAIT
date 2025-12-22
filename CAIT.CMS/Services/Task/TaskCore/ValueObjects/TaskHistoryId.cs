namespace TaskCore.ValueObjects
{
    public record TaskHistoryId
    {
        public Guid Value { get; }
        private TaskHistoryId(Guid value) => Value = value;

        public static TaskHistoryId Of(Guid value)
        {
            if (value == Guid.Empty) throw new DomainException("TaskHistoryId cannot be empty.");
            return new TaskHistoryId(value);
        }
    }
}
