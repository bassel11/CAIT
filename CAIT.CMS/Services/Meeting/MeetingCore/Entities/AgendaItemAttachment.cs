using MeetingCore.ValueObjects.AgendaItemAttachmentVO;
using MeetingCore.ValueObjects.AgendaItemVO;

namespace MeetingCore.Entities
{
    public class AgendaItemAttachment : Entity<AgendaItemAttachmentId>
    {
        // Explicit Foreign Key (Strongly Typed)
        public AgendaItemId AgendaItemId { get; private set; } = default!;

        // Value Object للملف
        public AttachmentFile FileDetails { get; private set; } = default!;

        public DateTime UploadedAt { get; private set; }

        private AgendaItemAttachment() { } // EF Core

        internal AgendaItemAttachment(AgendaItemId agendaItemId, AttachmentFile fileDetails)
        {
            Id = AgendaItemAttachmentId.Of(Guid.NewGuid());
            AgendaItemId = agendaItemId;
            FileDetails = fileDetails;
            UploadedAt = DateTime.UtcNow;
        }
    }
}
