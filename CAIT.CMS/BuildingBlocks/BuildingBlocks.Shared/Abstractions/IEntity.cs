namespace BuildingBlocks.Shared.Abstractions
{
    public interface IEntity<T> : IEntity, IHasDomainEvents
    {
        public T Id { get; set; }
    }

    public interface IEntity : IHasDomainEvents
    {
        public DateTime? CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? LastTimeModified { get; set; }
        public string? LastModifiedBy { get; set; }
    }
}
