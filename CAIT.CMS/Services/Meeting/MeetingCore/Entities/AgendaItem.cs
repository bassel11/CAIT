using MeetingCore.ValueObjects.AgendaItemAttachmentVO;
using MeetingCore.ValueObjects.AgendaItemVO;
using MeetingCore.ValueObjects.MeetingVO;

namespace MeetingCore.Entities
{
    public class AgendaItem : Entity<AgendaItemId>
    {
        public MeetingId MeetingId { get; private set; } = default!;
        public AgendaItemTitle Title { get; set; } = default!;
        public string? Description { get; set; }
        public Duration? AllocatedTime { get; private set; }
        public SortOrder SortOrder { get; private set; } = default!;
        public PresenterId? PresenterId { get; private set; }

        private readonly List<AgendaItemAttachment> _attachments = new();
        public IReadOnlyCollection<AgendaItemAttachment> Attachments => _attachments.AsReadOnly();

        private AgendaItem() { } // EF Core

        internal AgendaItem(
            MeetingId meetingId,
            AgendaItemTitle title,
            Duration? allocatedTime,
            SortOrder sortOrder,
            PresenterId? presenterId,
            string? description = null)
        {
            Id = AgendaItemId.Of(Guid.NewGuid());
            MeetingId = meetingId ?? throw new DomainException("MeetingId is required.");
            Title = title ?? throw new DomainException("Title is required.");
            AllocatedTime = allocatedTime;
            SortOrder = sortOrder ?? throw new DomainException("SortOrder is required.");
            PresenterId = presenterId;
            Description = description;
        }

        internal void Update(
            AgendaItemTitle title,
            string? description,
            SortOrder sortOrder,
            Duration? duration,
            PresenterId? presenterId)
        {
            Title = title;
            Description = description;
            SortOrder = sortOrder;
            AllocatedTime = duration;
            PresenterId = presenterId;
        }

        public void AddAttachment(string fileName, string fileUrl, string contentType)
        {
            var fileVO = AttachmentFile.Create(fileName, fileUrl, contentType);
            var attachment = new AgendaItemAttachment(this.Id, fileVO);
            _attachments.Add(attachment);
        }

        public void RemoveAttachment(AgendaItemAttachmentId attachmentId)
        {
            var attachment = _attachments.FirstOrDefault(a => a.Id == attachmentId);
            if (attachment != null) _attachments.Remove(attachment);
        }
    }
}
