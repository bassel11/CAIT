namespace TaskCore.ValueObjects
{
    public record TaskNoteId
    {
        public Guid Value { get; }

        private TaskNoteId(Guid value) => Value = value;

        //public static TaskNoteId New() => new TaskNoteId(Guid.NewGuid());

        public static TaskNoteId Of(Guid value)
        {
            if (value == Guid.Empty)
                throw new DomainException("TaskNoteId cannot be empty.");
            return new TaskNoteId(value);
        }

        //public override string ToString() => Value.ToString();
    }
}
