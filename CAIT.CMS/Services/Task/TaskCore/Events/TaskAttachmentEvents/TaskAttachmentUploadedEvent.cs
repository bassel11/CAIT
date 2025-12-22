namespace TaskCore.Events.TaskAttachmentEvents
{
    public record TaskAttachmentUploadedEvent(
        TaskItemId TaskId,
        TaskAttachmentId AttachmentId,
        UserId UploadedBy,
        string FileName,
        int Version,
        long SizeInBytes
    ) : IDomainEvent;
}
