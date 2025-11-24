namespace MeetingApplication.Features.Attendances.Queries.Results
{
    public class QuorumValidationResult
    {
        public Guid MeetingId { get; set; }
        public int TotalMembers { get; set; }               // from CommitteeService
        public int PresentCount { get; set; }               // attendees counted as Present or Remote
        public int RequiredCount { get; set; }              // required by rule
        public bool QuorumMet { get; set; }
        public string? RuleDescription { get; set; }        // human readable
        public string? Note { get; set; }
    }
}
