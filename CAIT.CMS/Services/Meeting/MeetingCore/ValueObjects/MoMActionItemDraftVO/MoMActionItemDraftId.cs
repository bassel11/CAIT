namespace MeetingCore.ValueObjects.MoMActionItemDraftVO
{
    public record MoMActionItemDraftId
    {
        public Guid Value { get; }

        private MoMActionItemDraftId(Guid value)
        {
            Value = value;
        }

        public static MoMActionItemDraftId Of(Guid value)
        {
            if (value == Guid.Empty)
                throw new DomainException("MoMActionItemDraftId cannot be empty.");

            return new MoMActionItemDraftId(value);
        }

        //public override string ToString() => Value.ToString();
    }

}
