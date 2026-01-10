namespace TaskApplication.Data
{
    public interface IApplicationDbContext
    {
        DbSet<TaskItem> TaskItems { get; }
        DbSet<TaskAssignee> TaskAssignees { get; }
        DbSet<TaskNote> TaskNotes { get; }
        DbSet<TaskAttachment> TaskAttachments { get; }
        DbSet<TaskHistory> TaskHistories { get; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
