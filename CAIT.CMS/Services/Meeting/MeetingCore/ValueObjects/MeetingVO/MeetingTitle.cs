using BuildingBlocks.Shared.Exceptions;

namespace MeetingCore.ValueObjects.MeetingVO
{
    public record MeetingTitle
    {
        public string Value { get; }

        private MeetingTitle(string value)
        {
            Value = value;
        }

        public static MeetingTitle Of(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new DomainException("Title cannot be empty.");

            // إذا أردت، يمكن إضافة تحقق لطول محدد
            // if (value.Length > 100) throw new ArgumentException("Title too long.");

            return new MeetingTitle(value);
        }

        public override string ToString() => Value;
    }

}
