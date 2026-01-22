namespace MeetingCore.ValueObjects.AgendaItemVO
{
    public record AgendaItemId
    {
        public Guid Value { get; }

        private AgendaItemId(Guid value) => Value = value;

        //public static TaskAssigneeId New() => new TaskAssigneeId(Guid.NewGuid());

        public static AgendaItemId Of(Guid value)
        {
            if (value == Guid.Empty)
                throw new DomainException("AgendaItemId cannot be empty.");
            return new AgendaItemId(value);
        }

        //public override string ToString() => Value.ToString();
    }
}
