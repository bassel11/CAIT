namespace Monitoring.Core.Entities
{
    public class CommitteeSummary
    {
        public Guid Id { get; set; } // CommitteeId
        public string Name { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty; // Active, Suspended...
        public DateTime CreatedAt { get; set; }
        public DateTime? LastActivityDate { get; set; } // لتحديد اللجان الخاملة
        public int MemberCount { get; set; }
        public int CompletedTasksCount { get; set; }
        public int OverdueTasksCount { get; set; }
        public double AttendanceRate { get; set; } // نسبة مئوية
        public bool IsCompliant { get; set; } = true;
        public string? NonComplianceReason { get; set; }
    }
}
