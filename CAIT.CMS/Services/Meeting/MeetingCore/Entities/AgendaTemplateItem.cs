using MeetingCore.ValueObjects.AgendaTemplateVO;

namespace MeetingCore.Entities
{
    public class AgendaTemplateItem : Entity<AgendaTemplateItemId>
    {
        // FK صريح (Strongly Typed)
        public AgendaTemplateId AgendaTemplateId { get; private set; } = default!;

        public string Title { get; private set; } = default!;
        public string? Description { get; private set; }
        public int DurationMinutes { get; private set; }
        public int SortOrder { get; private set; }

        private AgendaTemplateItem() { }

        internal AgendaTemplateItem(AgendaTemplateId templateId, string title, int durationMinutes, string? description, int sortOrder)
        {
            Id = AgendaTemplateItemId.Of(Guid.NewGuid());
            AgendaTemplateId = templateId;
            Title = title;
            DurationMinutes = durationMinutes;
            Description = description;
            SortOrder = sortOrder;
        }
    }
}
