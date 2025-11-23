using MeetingCore.Enums;
using System.ComponentModel.DataAnnotations;

namespace MeetingCore.Entities
{
    public class Meeting
    {
        public Guid Id { get; set; }

        [Required]
        public Guid CommitteeId { get; set; }

        public string Title { get; set; } = default!;
        public string? Description { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public MeetingStatus Status { get; set; }  // Scheduled, Rescheduled, Cancelled, Completed

        public bool IsRecurring { get; set; }
        public RecurrenceType RecurrenceType { get; set; } // None, Weekly, Monthly, Quarterly
        public string? RecurrenceRule { get; set; }

        public string? TeamsLink { get; set; }
        public string? OutlookEventId { get; set; }

        public Guid CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid? UpdatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // Concurrency token
        public byte[]? RowVersion { get; set; }

        public ICollection<AgendaItem> AgendaItems { get; set; } = new List<AgendaItem>();
        public ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();
        public MinutesOfMeeting? Minutes { get; set; }
        public ICollection<MeetingDecision> Decisions { get; set; } = new List<MeetingDecision>();
        public ICollection<AIGeneratedContent> AIGeneratedContents { get; set; } = new List<AIGeneratedContent>();
        public ICollection<MeetingIntegrationLog> IntegrationLogs { get; set; } = new List<MeetingIntegrationLog>();
        public ICollection<MeetingNotification> Notifications { get; set; } = new List<MeetingNotification>();
    }
}
