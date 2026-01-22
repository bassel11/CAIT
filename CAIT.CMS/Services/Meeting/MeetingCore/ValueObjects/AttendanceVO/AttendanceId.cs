namespace MeetingCore.ValueObjects.AttendanceVO
{
    public record AttendanceId
    {
        public Guid Value { get; }

        private AttendanceId(Guid value) => Value = value;

        //public static AttendanceId New() => new AttendanceId(Guid.NewGuid());

        public static AttendanceId Of(Guid value)
        {
            if (value == Guid.Empty)
                throw new DomainException("AttendanceId cannot be empty.");
            return new AttendanceId(value);
        }

        //public override string ToString() => Value.ToString();
    }
}
