namespace CommitteeCore.Entities
{
    public class CommitteeStatus
    {
        public int Id { get; set; }                 // 1=Active, 2=Suspended ...
        public string Name { get; set; }            // Active, Suspended, Completed...
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<Committee> Committees { get; set; }
        // History where this status was the previous state
        public ICollection<CommitteeStatusHistory> OldStatusHistories { get; set; }

        // History where this status became the new state
        public ICollection<CommitteeStatusHistory> NewStatusHistories { get; set; }
    }
}
