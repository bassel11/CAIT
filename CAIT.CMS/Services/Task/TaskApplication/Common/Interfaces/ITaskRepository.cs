using TaskCore.Entities;
using TaskCore.ValueObjects;

namespace TaskApplication.Common.Interfaces
{
    public interface ITaskRepository
    {
        Task<TaskItem?> GetByIdAsync(TaskItemId id, CancellationToken cancellationToken = default);
        Task AddAsync(TaskItem task, CancellationToken cancellationToken = default);
        Task UpdateAsync(TaskItem task, CancellationToken cancellationToken = default);
        Task SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
