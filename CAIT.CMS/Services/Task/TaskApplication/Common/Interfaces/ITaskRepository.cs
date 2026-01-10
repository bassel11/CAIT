namespace TaskApplication.Common.Interfaces
{
    public interface ITaskRepository
    {
        Task<TaskItem?> GetByIdAsync(TaskItemId id, CancellationToken cancellationToken = default);
        Task AddAsync(TaskItem task, CancellationToken cancellationToken = default);
        Task UpdateAsync(TaskItem task, CancellationToken cancellationToken = default);
        Task SaveChangesAsync(CancellationToken cancellationToken = default);

        // جلب المهام المتأخرة التي لم تصبح حالتها Overdue بعد
        Task<List<TaskItem>> GetOverdueCandidatesAsync(CancellationToken cancellationToken);

        // جلب المهام التي موعدها بعد X أيام (للتذكير)
        // daysUntilDeadline: 1, 3, or 7
        Task<List<TaskItem>> GetReminderCandidatesAsync(int daysUntilDeadline, CancellationToken cancellationToken);
    }
}
