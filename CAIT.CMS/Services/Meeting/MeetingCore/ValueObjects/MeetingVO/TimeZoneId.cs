using BuildingBlocks.Shared.Exceptions;

namespace MeetingCore.ValueObjects.MeetingVO
{
    public sealed record TimeZoneId
    {
        public string Value { get; }

        private TimeZoneId(string value)
        {
            Value = value;
        }

        public static TimeZoneId Of(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new DomainException("TimeZoneId cannot be empty.");

            // Validate against system time zones
            try
            {
                _ = TimeZoneInfo.FindSystemTimeZoneById(value);
            }
            catch (TimeZoneNotFoundException)
            {
                throw new DomainException($"Invalid TimeZoneId: '{value}'.");
            }
            catch (InvalidTimeZoneException)
            {
                throw new DomainException($"Corrupted TimeZoneId: '{value}'.");
            }

            return new TimeZoneId(value);
        }

        public static TimeZoneId Utc => new("UTC");

        public override string ToString() => Value;
    }
}
