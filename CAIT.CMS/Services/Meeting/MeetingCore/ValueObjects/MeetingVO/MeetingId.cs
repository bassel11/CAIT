using BuildingBlocks.Shared.Exceptions;

namespace MeetingCore.ValueObjects.MeetingVO
{
    public record MeetingId
    {
        public Guid Value { get; }
        private MeetingId(Guid value)
        {
            Value = value;
        }
        public static MeetingId Of(Guid value)
        {
            if (value == Guid.Empty)
                throw new DomainException("MeetingId cannot be empty.");

            return new MeetingId(value);
        }

        //public static MeetingId New() => new MeetingId(Guid.NewGuid());

        //public override string ToString() => Value.ToString();
    }
}
