using BuildingBlocks.Shared.Exceptions;
using MeetingCore.Enums.MeetingEnums;

namespace MeetingCore.ValueObjects.MeetingVO
{
    public sealed record RecurrencePattern
    {
        public bool IsRecurring { get; }
        public RecurrenceType Type { get; }
        public string? Rule { get; }

        private RecurrencePattern(bool isRecurring, RecurrenceType type, string? rule)
        {
            IsRecurring = isRecurring;
            Type = type;
            Rule = rule;
        }

        public static RecurrencePattern None =>
            new(false, RecurrenceType.None, null);

        public static RecurrencePattern Simple(RecurrenceType type)
        {
            if (type == RecurrenceType.None)
                throw new DomainException("Recurrence type cannot be None for recurring meetings.");

            return new(true, type, null);
        }

        public static RecurrencePattern WithRule(string rule)
        {
            if (string.IsNullOrWhiteSpace(rule))
                throw new DomainException("Recurrence rule cannot be empty.");

            return new(true, RecurrenceType.None, rule);
        }
    }
}
