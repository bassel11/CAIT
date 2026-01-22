namespace MeetingApplication.Features.MoMDecisionDrafts.Queries.Results
{
    public class MoMDecisionResponse
    {
        public string Title { get; set; } = default!;
        public string Text { get; set; } = default!;
        public int SortOrder { get; set; }
        public string Status { get; set; } = default!;
    }
}
