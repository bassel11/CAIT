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


        public async Task<List<TaskItem>> GetOverdueCandidatesAsync(CancellationToken cancellationToken)
        {
            //var now = DateTime.UtcNow;

            // ننشئ كائناً يمثل "الآن" لكي نقارنه مع الكائنات في الداتابيز
            var nowDeadline = TaskDeadline.FromDatabase(DateTime.UtcNow);

            return await _context.TaskItems
                .Include(t => t.TaskAssignees)
                .Where(t =>
                    t.Status != TaskCore.Enums.TaskStatus.Completed &&
                    t.Status != TaskCore.Enums.TaskStatus.Cancelled &&
                    t.Status != TaskCore.Enums.TaskStatus.Overdue &&
                    t.Deadline != null &&
                    t.Deadline < nowDeadline) // ✅ الآن هذا سيعمل 100% ويترجم لـ SQL
                .ToListAsync(cancellationToken);
        }

        //public async Task<List<TaskItem>> GetReminderCandidatesAsync(int daysUntilDeadline, CancellationToken cancellationToken)
        //{
        //    var targetDate = DateTime.UtcNow.Date.AddDays(daysUntilDeadline);

        //    // نبحث عن المهام التي موعدها يطابق تاريخ اليوم + X أيام
        //    return await _context.TaskItems
        //        .Include(t => t.TaskAssignees)
        //        .Where(t =>
        //            t.Status != TaskCore.Enums.TaskStatus.Completed &&
        //            t.Status != TaskCore.Enums.TaskStatus.Cancelled &&
        //            t.Deadline != null &&
        //            t.Deadline.Value.Date == targetDate) // مطابقة التاريخ فقط (تجاهل الوقت)
        //        .ToListAsync(cancellationToken);
        //}

        public async Task<List<TaskItem>> GetReminderCandidatesAsync(int daysUntilDeadline, CancellationToken cancellationToken)
        {
            // 1. تحديد بداية اليوم ونهايته (بداية اليوم التالي)
            var targetDateStart = DateTime.UtcNow.Date.AddDays(daysUntilDeadline); // 00:00:00
            var targetDateEnd = targetDateStart.AddDays(1); // 00:00:00 اليوم التالي

            // 2. تحويل التواريخ إلى Value Objects للمقارنة
            var startDeadline = TaskDeadline.FromDatabase(targetDateStart);
            var endDeadline = TaskDeadline.FromDatabase(targetDateEnd);

            return await _context.TaskItems
                .Include(t => t.TaskAssignees)
                .Where(t =>
                    t.Status != TaskCore.Enums.TaskStatus.Completed &&
                    t.Status != TaskCore.Enums.TaskStatus.Cancelled &&
                    t.Deadline != null &&

                    // ✅ الحل الاحترافي: استخدام النطاق
                    // هذا يترجم في SQL إلى: Deadline >= '...' AND Deadline < '...'
                    t.Deadline >= startDeadline &&
                    t.Deadline < endDeadline)
                .ToListAsync(cancellationToken);
        }

        public async Task<bool> TasksExistForMoMAsync(MoMId momId)
        {
            return await _context.TaskItems.AnyAsync(t => t.MoMId == momId);
        }
    }
}
