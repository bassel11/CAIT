namespace MeetingApplication.Features.MoMActionItemDrafts.Queries.Results
{
    public class MoMActionItemResponse
    {
        public string TaskTitle { get; set; } = default!;
        public Guid? AssigneeId { get; set; }
        public DateTime? DueDate { get; set; }
        public int SortOrder { get; set; }
        public string Status { get; set; } = default!;
    }
}
