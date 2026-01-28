namespace MeetingCore.ValueObjects.AgendaTemplateVO
{
    public record AgendaTemplateId
    {
        public Guid Value { get; }
        private AgendaTemplateId(Guid value) => Value = value;

        public static AgendaTemplateId Of(Guid value)
        {
            if (value == Guid.Empty)
                throw new DomainException("AgendaTemplateId cannot be empty.");
            return new AgendaTemplateId(value);
        }
    }
}
