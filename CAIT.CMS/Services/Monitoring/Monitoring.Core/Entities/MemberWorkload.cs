namespace Monitoring.Core.Entities
{
    public class MemberWorkload
    {
        public Guid MemberId { get; set; }
        public string MemberName { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public int TotalCommittees { get; set; }
        public int PendingTasks { get; set; }
        public int OverdueTasks { get; set; }

        // خاصية لحساب المخاطر (AI logic logic logic goes here conceptually)
        public bool IsOverloaded => TotalCommittees > 5 || PendingTasks > 10;
    }
}
