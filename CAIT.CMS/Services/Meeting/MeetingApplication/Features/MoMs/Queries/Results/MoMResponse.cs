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

        public List<MoMAttendanceDto> AttendanceList { get; set; } = new();
        public List<MoMDiscussionDto> Discussions { get; set; } = new();
        public List<DecisionDto> Decisions { get; set; } = new();
        public List<ActionItemDto> ActionItems { get; set; } = new();
    }

    public record MoMAttendanceDto
    {
        public Guid Id { get; init; }
        public string MemberName { get; init; } = default!;
        public string Role { get; init; } = default!;
        public bool IsPresent { get; init; }
        public string Status { get; init; } = default!;
        public string? Notes { get; init; }
    }

    public record MoMDiscussionDto
    {
        public Guid Id { get; init; }
        public string TopicTitle { get; init; } = default!;
        public string Content { get; init; } = default!;
        public Guid? OriginalAgendaItemId { get; init; }
    }

    public record DecisionDto(Guid Id, string Title, string Text);

    public record ActionItemDto(Guid Id, string TaskTitle, Guid? AssigneeId);
}
