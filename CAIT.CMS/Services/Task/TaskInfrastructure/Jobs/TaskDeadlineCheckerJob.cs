//using MassTransit;
//using Microsoft.Extensions.Logging;
//using Quartz;

//namespace TaskInfrastructure.Jobs
//{
//    public class TaskDeadlineCheckerJob : IJob
//    {
//        private readonly ApplicationDbContext _dbContext;
//        private readonly IPublishEndpoint _publishEndpoint;
//        private readonly ILogger<TaskDeadlineCheckerJob> _logger;

//        public TaskDeadlineCheckerJob(ApplicationDbContext dbContext, IPublishEndpoint publishEndpoint, ILogger<TaskDeadlineCheckerJob> logger)
//        {
//            _dbContext = dbContext;
//            _publishEndpoint = publishEndpoint;
//            _logger = logger;
//        }

//        public async Task Execute(IJobExecutionContext context)
//        {
//            var now = DateTime.UtcNow;

//            // 1. البحث عن المهام المتأخرة (Overdue)
//            var overdueTasks = await _dbContext.TaskItems
//                .Include(t => t.Assignments)
//                .Where(t => t.Status != Domain.Entities.TaskStatus.Completed
//                         && t.Status != Domain.Entities.TaskStatus.Overdue
//                         && t.Deadline < now)
//                .ToListAsync();

//            foreach (var task in overdueTasks)
//            {
//                // تحديث الحالة
//                task.CheckOverdue();

//                // حساب أيام التأخير
//                var daysOverdue = (now - task.Deadline.Value).Days;

//                // إرسال حدث التصعيد (Notification Service ستتولى إرسال الإيميل للـ Chairman)
//                await _publishEndpoint.Publish(new TaskOverdueEscalationEvent(
//                    task.Id,
//                    task.Title,
//                    daysOverdue,
//                    task.Assignments.FirstOrDefault()?.UserId ?? Guid.Empty,
//                    task.MeetingId // يمكن استخدامه لجلب رئيس اللجنة
//                ));
//            }

//            // 2. إرسال التذكيرات (Reminders) - 3 أيام قبل الموعد مثلاً
//            var upcomingTasks = await _dbContext.Tasks
//                .Include(t => t.Assignments)
//                .Where(t => t.Status != Domain.Entities.TaskStatus.Completed
//                         && t.Deadline > now
//                         && t.Deadline <= now.AddDays(3)) // تذكير قبل 3 أيام
//                .ToListAsync();

//            foreach (var task in upcomingTasks)
//            {
//                // إرسال حدث تذكير (ليس تصعيد)
//                // await _publishEndpoint.Publish(new TaskReminderEvent(...));
//            }

//            await _dbContext.SaveChangesAsync();
//        }
//    }
//}
