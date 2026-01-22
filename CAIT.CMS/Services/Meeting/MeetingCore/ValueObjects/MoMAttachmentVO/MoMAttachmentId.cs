namespace MeetingCore.ValueObjects.MoMAttachmentVO
{
    public record MoMAttachmentId
    {
        public Guid Value { get; }
        private MoMAttachmentId(Guid value)
        {
            Value = value;
        }
        public static MoMAttachmentId Of(Guid value)
        {
            if (value == Guid.Empty)
                throw new DomainException("MoMAttachmentId cannot be empty.");

            return new MoMAttachmentId(value);
        }

        //public static MoMAttachmentId New() => new MoMAttachmentId(Guid.NewGuid());

        //public override string ToString() => Value.ToString();
    }
}
