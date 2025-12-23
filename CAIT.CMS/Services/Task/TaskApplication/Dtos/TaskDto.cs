namespace TaskApplication.Dtos
{
    public record TaskDto(
        Guid Id,
        string Title,
        string Description,
        string Status,       // e.g., "In Progress", "Completed"
        string Priority,     // e.g., "High", "Critical"
        string Category,     // e.g., "Strategic", "Operational"
        DateTime? Deadline,
        bool IsOverdue,      // خاصية محسوبة لتسهيل العرض في الـ UI (لون أحمر مثلاً)

        // المراجع (References)
        Guid CommitteeId,
        Guid? MeetingId,
        Guid? DecisionId,
        Guid? MoMId,

        // التواريخ الهامة
        DateTime CreatedAt,
        DateTime? LastModifiedAt,

        // القوائم الفرعية (Nested Data)
        List<TaskAssigneeDto> Assignees,
        List<TaskAttachmentDto> Attachments,
        List<TaskNoteDto> Notes,
        List<TaskHistoryDto> History // سجل النشاطات (Audit Trail)
    );

    //public record TaskAssigneeDto(Guid UserId, string Name, string Email);
}
