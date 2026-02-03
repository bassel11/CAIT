namespace MeetingCore.ValueObjects.MoMDiscussionVO
{
    public record MoMDiscussionId
    {
        public Guid Value { get; }
        private MoMDiscussionId(Guid value)
        {
            Value = value;
        }
        public static MoMDiscussionId Of(Guid value)
        {
            if (value == Guid.Empty)
                throw new DomainException("MoMDiscussionId cannot be empty.");

            return new MoMDiscussionId(value);
        }

        //public static MoMDiscussionId New() => new MoMDiscussionId(Guid.NewGuid());

        //public override string ToString() => Value.ToString();
    }
}
