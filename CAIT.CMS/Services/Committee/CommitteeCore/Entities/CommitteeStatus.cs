namespace CommitteeCore.Entities
{
    public class CommitteeStatus
    {
        public int Id { get; set; }                 // 1=Active, 2=Suspended ...
        public string Name { get; set; }            // Active, Suspended, Completed...
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<Committee> Committees { get; set; }
        public ICollection<CommitteeStatusHistory> CommitteeStatusHistories { get; set; }
    }
}
