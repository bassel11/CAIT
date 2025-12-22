namespace TaskCore.Events.TaskNoteEvents
{
    public record TaskNoteRemovedEvent(
        TaskItemId TaskId,
        TaskNoteId NoteId,
        UserId RemovedByUserId,
        string DeletedContent // ✅ نحتفظ بالمحتوى المحذوف للأرشيف
    ) : IDomainEvent;
}
