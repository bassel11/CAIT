namespace MeetingCore.ValueObjects.AgendaItemAttachmentVO
{
    public record AgendaItemAttachmentId
    {
        public Guid Value { get; }
        private AgendaItemAttachmentId(Guid value) => Value = value;

        public static AgendaItemAttachmentId Of(Guid value)
        {
            if (value == Guid.Empty)
                throw new DomainException("AgendaItemAttachmentId cannot be empty.");
            return new AgendaItemAttachmentId(value);
        }
    }
}
