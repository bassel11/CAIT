namespace MeetingCore.ValueObjects.AgendaTemplateVO
{
    public record AgendaTemplateItemId
    {
        public Guid Value { get; }
        private AgendaTemplateItemId(Guid value) => Value = value;

        public static AgendaTemplateItemId Of(Guid value)
        {
            if (value == Guid.Empty)
                throw new DomainException("AgendaTemplateItemId cannot be empty.");
            return new AgendaTemplateItemId(value);
        }
    }
}
