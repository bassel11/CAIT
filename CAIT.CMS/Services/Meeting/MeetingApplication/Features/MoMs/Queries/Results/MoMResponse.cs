namespace MeetingApplication.Features.MoMs.Queries.Results
{
    public class MoMResponse
    {
        public Guid Id { get; set; }
        public Guid MeetingId { get; set; }
        public string Status { get; set; } = default!;
        public string ContentHtml { get; set; } = default!;
        public int Version { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? LastModified { get; set; }
        public Guid? ApprovedBy { get; set; }

        // إحصائيات سريعة (اختياري ومفيد للواجهة)
        public int DecisionsCount { get; set; }
        public int ActionItemsCount { get; set; }
        public int AttachmentsCount { get; set; }
    }
}
