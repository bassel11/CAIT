namespace MeetingCore.ValueObjects.AIGeneratedContentVO
{
    public record AIContentId
    {
        public Guid Value { get; }

        private AIContentId(Guid value) => Value = value;

        //public static AIContentId New() => new AIContentId(Guid.NewGuid());

        public static AIContentId Of(Guid value)
        {
            if (value == Guid.Empty)
                throw new DomainException("AIContentId cannot be empty.");
            return new AIContentId(value);
        }

        //public override string ToString() => Value.ToString();
    }


}
