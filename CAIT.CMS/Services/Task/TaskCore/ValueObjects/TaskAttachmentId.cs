namespace TaskCore.ValueObjects
{
    public record TaskAttachmentId
    {
        public Guid Value { get; }

        private TaskAttachmentId(Guid value) => Value = value;

        public static TaskAttachmentId Of(Guid value)
        {
            if (value == Guid.Empty)
                throw new DomainException("TaskAttachmentId cannot be empty.");
            return new TaskAttachmentId(value);
        }
    }
}
