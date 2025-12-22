namespace TaskCore.Events.TaskNoteEvents
{
    public record TaskNoteUpdatedEvent(
        TaskItemId TaskId,
        TaskNoteId NoteId,
        UserId UpdatedByUserId,
        string OldContent, // ✅ هام جداً للـ Audit Trail
        string NewContent
    ) : IDomainEvent;
}
