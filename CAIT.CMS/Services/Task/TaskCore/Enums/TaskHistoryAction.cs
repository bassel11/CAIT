namespace TaskCore.Enums
{
    public enum TaskHistoryAction
    {
        Created = 1,
        Updated = 2,
        StatusChanged = 3,
        PriorityChanged = 4,
        DeadlineChanged = 5,
        Assigned = 6,
        Unassigned = 7,
        NoteAdded = 8,
        AttachmentUploaded = 9,
        AttachmentRemoved = 10
    }
}
