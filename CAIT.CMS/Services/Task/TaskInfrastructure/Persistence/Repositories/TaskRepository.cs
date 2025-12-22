using TaskApplication.Common.Interfaces;

namespace TaskInfrastructure.Persistence.Repositories
{
    public class TaskRepository : ITaskRepository
    {
        private readonly ApplicationDbContext _context;

        public TaskRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<TaskItem?> GetByIdAsync(TaskItemId id, CancellationToken cancellationToken = default)
        {
            return await _context.TaskItems
                .Include(t => t.TaskAssignees)
                .Include(t => t.TaskNotes)
                .Include(t => t.TaskAttachments)
                .Include(t => t.TaskHistories) // Be careful loading history if large
                .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
        }

        public async Task AddAsync(TaskItem task, CancellationToken cancellationToken = default)
        {
            await _context.TaskItems.AddAsync(task, cancellationToken);
            // SaveChanges is usually called by the UnitOfWork or CommandHandler
        }

        public Task UpdateAsync(TaskItem task, CancellationToken cancellationToken = default)
        {
            _context.TaskItems.Update(task);
            return Task.CompletedTask;
        }

        public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
