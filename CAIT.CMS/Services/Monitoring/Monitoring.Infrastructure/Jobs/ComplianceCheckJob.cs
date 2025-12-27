using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Monitoring.Infrastructure.Data;
using Quartz;

namespace Monitoring.Infrastructure.Jobs
{
    [DisallowConcurrentExecution]
    public class ComplianceCheckJob : IJob
    {
        private readonly MonitoringDbContext _context;
        private readonly ILogger<ComplianceCheckJob> _logger;

        public ComplianceCheckJob(MonitoringDbContext context, ILogger<ComplianceCheckJob> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            _logger.LogInformation("Starting Compliance Check Job...");

            var sixMonthsAgo = DateTime.UtcNow.AddMonths(-6);

            // 1. تحديد اللجان الخاملة
            var dormantCommittees = await _context.CommitteeSummaries
                .Where(c => c.Status == "Active" && c.LastActivityDate < sixMonthsAgo)
                .ToListAsync();

            foreach (var committee in dormantCommittees)
            {
                committee.IsCompliant = false;
                committee.NonComplianceReason = "Dormant: No activity for 6 months.";
                // هنا يمكن إرسال حدث (Integration Event) لإشعار المدير
            }

            await _context.SaveChangesAsync();
            _logger.LogInformation($"Compliance Check Completed. Flagged {dormantCommittees.Count} committees.");
        }
    }
}
