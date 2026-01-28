using MeetingCore.ValueObjects.AgendaTemplateVO;

namespace MeetingCore.Entities
{
    public class AgendaTemplate : Aggregate<AgendaTemplateId>
    {
        public string Name { get; private set; } = default!;
        public string Description { get; private set; } = default!;
        public bool IsActive { get; private set; } = true;

        // علاقة 1-to-Many صريحة
        private readonly List<AgendaTemplateItem> _items = new();
        public IReadOnlyCollection<AgendaTemplateItem> Items => _items.AsReadOnly();

        private AgendaTemplate() { }

        public AgendaTemplate(AgendaTemplateId id, string name, string description, string createdBy)
        {
            Id = id;
            Name = name;
            Description = description;
            CreatedBy = createdBy;
            CreatedAt = DateTime.UtcNow;
        }

        public void AddItem(string title, int durationMinutes, string? description, int sortOrder)
        {
            _items.Add(new AgendaTemplateItem(Id, title, durationMinutes, description, sortOrder));
        }

        // داخل AgendaTemplate.cs
        public void UpdateDetails(string name, string description)
        {
            Name = name;
            Description = description;
        }

        public void ClearItems()
        {
            _items.Clear();
        }
    }
}
