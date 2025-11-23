using MeetingCore.Enums;

namespace MeetingCore.Entities
{
    public class AIGeneratedContent
    {
        public Guid Id { get; set; }

        public Guid MeetingId { get; set; }
        public Meeting Meeting { get; set; } = null!;

        public AIContentType ContentType { get; set; } // AgendaDraft, MoMDraft
        public string Prompt { get; set; } = null!;
        public string GeneratedText { get; set; } = null!;

        public DateTime CreatedAt { get; set; }
        public Guid CreatedBy { get; set; }
    }
}
