using Microsoft.EntityFrameworkCore;
using NotificationService.Data;
using Quartz;

namespace NotificationService.Jobs
{
    [DisallowConcurrentExecution]
    public class NotificationCleanupJob : IJob
    {
        private readonly NotificationDbContext _context;
        private readonly ILogger<NotificationCleanupJob> _logger;

        public NotificationCleanupJob(NotificationDbContext context, ILogger<NotificationCleanupJob> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            _logger.LogInformation("🧹 Starting Notification Cleanup Job...");

            // حذف الإشعارات الأقدم من 30 يوماً
            var cutoffDate = DateTime.UtcNow.AddDays(-30);

            var deletedCount = await _context.AppNotifications
                .Where(n => n.CreatedAt < cutoffDate)
                .ExecuteDeleteAsync();

            _logger.LogInformation("✅ Cleanup Complete. Deleted {count} old notifications.", deletedCount);
        }
    }
}
