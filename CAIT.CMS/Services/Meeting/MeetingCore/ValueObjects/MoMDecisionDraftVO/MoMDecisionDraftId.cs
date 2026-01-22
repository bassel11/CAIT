namespace MeetingCore.ValueObjects.MoMDecisionDraftVO
{
    public record MoMDecisionDraftId
    {
        public Guid Value { get; }

        private MoMDecisionDraftId(Guid value)
        {
            Value = value;
        }

        public static MoMDecisionDraftId Of(Guid value)
        {
            if (value == Guid.Empty)
                throw new DomainException("MoMDecisionDraftId cannot be empty.");

            return new MoMDecisionDraftId(value);
        }

        //public override string ToString() => Value.ToString();
    }
}
