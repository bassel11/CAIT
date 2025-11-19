namespace CommitteeApplication.Features.StatusHistories.Queries.Results
{
    public class CommitStatusHistoryResponse
    {
        public Guid Id { get; set; }
        public Guid CommitteeId { get; set; }
        public int OldStatusId { get; set; }
        public string OldStatusName { get; set; } = string.Empty;
        public int NewStatusId { get; set; }
        public string NewStatusName { get; set; } = string.Empty;
        public DateTime ChangedAt { get; set; }
        public string DecisionText { get; set; } = string.Empty;
        public string DecisionDocumentUrl { get; set; } = string.Empty;
    }
}
