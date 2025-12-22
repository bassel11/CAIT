namespace TaskCore.ValueObjects
{
    public record TaskItemId
    {
        public Guid Value { get; }
        private TaskItemId(Guid value)
        {
            Value = value;
        }
        public static TaskItemId Of(Guid value)
        {
            if (value == Guid.Empty)
                throw new DomainException("TaskItemId cannot be empty.");

            return new TaskItemId(value);
        }

        //public static TaskItemId New() => new TaskItemId(Guid.NewGuid());

        //public override string ToString() => Value.ToString();
    }
}
