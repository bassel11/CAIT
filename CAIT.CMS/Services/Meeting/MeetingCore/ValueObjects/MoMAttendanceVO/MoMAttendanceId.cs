namespace MeetingCore.ValueObjects.MoMAttendanceVO
{
    public record MoMAttendanceId
    {
        public Guid Value { get; }
        private MoMAttendanceId(Guid value)
        {
            Value = value;
        }
        public static MoMAttendanceId Of(Guid value)
        {
            if (value == Guid.Empty)
                throw new DomainException("MoMAttendanceId cannot be empty.");

            return new MoMAttendanceId(value);
        }

        //public static MoMAttendanceId New() => new MoMAttendanceId(Guid.NewGuid());

        //public override string ToString() => Value.ToString();
    }
}
