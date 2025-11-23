using MeetingCore.Enums;

namespace MeetingCore.Entities
{
    public class MeetingIntegrationLog
    {
        public Guid Id { get; set; }

        public Guid MeetingId { get; set; }
        public Meeting Meeting { get; set; } = null!;

        public IntegrationType IntegrationType { get; set; } // OutlookCreate, OutlookUpdate, TeamsCreate
        public bool Success { get; set; }

        public string? ExternalId { get; set; }
        public string? ErrorMessage { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
