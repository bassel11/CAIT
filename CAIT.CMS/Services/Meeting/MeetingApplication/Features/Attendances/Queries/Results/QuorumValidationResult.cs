namespace MeetingApplication.Features.Attendances.Queries.Results
{
    public class QuorumValidationResult
    {
        public Guid MeetingId { get; set; }
        public int TotalVotingMembers { get; set; } // تم تغيير الاسم ليكون أدق (المصوتون داخل الاجتماع)
        public int PresentVotingMembers { get; set; }
        public int RequiredCount { get; set; }
        public bool QuorumMet { get; set; }
        public string RuleDescription { get; set; } = default!;
        public string StatusMessage { get; set; } = default!;
    }
}
