namespace TaskCore.Events.TaskNoteEvents
{
    public record TaskNoteAddedEvent(
        TaskItemId TaskId,
        UserId AddedByUserId,
        string Content
    ) : IDomainEvent;
}
