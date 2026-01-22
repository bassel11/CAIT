namespace BuildingBlocks.Shared.Abstractions
{
    public abstract class Entity<T> : IEntity<T>, IHasDomainEvents
    {
        public T Id { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? LastTimeModified { get; set; }
        public string? LastModifiedBy { get; set; }
        //public bool IsDeleted { get; set; }
        //public DateTime? DeletedAt { get; set; }
        //public string? DeletedBy { get; set; }

        private readonly List<IDomainEvent> _domainEvents = new();
        public IReadOnlyList<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

        public void AddDomainEvent(IDomainEvent domainEvent)
        {
            _domainEvents.Add(domainEvent);
        }

        public IDomainEvent[] ClearDomainEvents()
        {
            var dequeuedEvents = _domainEvents.ToArray();
            _domainEvents.Clear();
            return dequeuedEvents;
        }


    }
}
