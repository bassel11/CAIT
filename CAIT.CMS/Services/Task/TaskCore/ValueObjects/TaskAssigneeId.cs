namespace TaskCore.ValueObjects
{
    public record TaskAssigneeId
    {
        public Guid Value { get; }

        private TaskAssigneeId(Guid value) => Value = value;

        //public static TaskAssigneeId New() => new TaskAssigneeId(Guid.NewGuid());

        public static TaskAssigneeId Of(Guid value)
        {
            if (value == Guid.Empty)
                throw new DomainException("TaskAssigneeId cannot be empty.");
            return new TaskAssigneeId(value);
        }

        //public override string ToString() => Value.ToString();
    }
}
