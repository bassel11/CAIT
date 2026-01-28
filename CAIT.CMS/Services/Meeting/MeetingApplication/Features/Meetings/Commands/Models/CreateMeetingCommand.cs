using BuildingBlocks.Shared.CQRS;
using BuildingBlocks.Shared.Wrappers;
using MeetingCore.Enums.MeetingEnums;

namespace MeetingApplication.Features.Meetings.Commands.Models
{
    public record CreateMeetingCommand : ICommand<Result<Guid>>
    {
        public Guid CommitteeId { get; init; }
        public string Title { get; init; }
        public string? Description { get; init; }

        public DateTime StartDate { get; init; }
        public DateTime EndDate { get; init; }
        public string TimeZone { get; init; }

        public LocationType LocationType { get; init; }
        public string? LocationRoom { get; init; }
        public string? LocationAddress { get; init; }
        public string? LocationOnlineUrl { get; init; }

        public bool IsRecurring { get; init; } = false;
        public RecurrenceType? RecurrenceType { get; init; } = MeetingCore.Enums.MeetingEnums.RecurrenceType.None;
        public string? RecurrenceRule { get; init; }
        public bool AutoAddMembers { get; init; } = true; // من اجل جلب الاعضاء تلقائيا من خدمة الجان
    }
}
