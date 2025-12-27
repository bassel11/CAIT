using Microsoft.EntityFrameworkCore;
using Monitoring.Application.Dtos;
using Monitoring.Application.Interfaces;
using Monitoring.Infrastructure.Data;

namespace Monitoring.Infrastructure.Persistence
{
    public class MonitoringRepository : IMonitoringRepository
    {
        private readonly MonitoringDbContext _context;

        public MonitoringRepository(MonitoringDbContext context)
        {
            _context = context;
        }

        public async Task<DashboardStatsDto> GetCommitteeStatisticsAsync()
        {
            // تنفيذ منطق التجميع هنا
            var stats = await _context.CommitteeSummaries
                .GroupBy(c => 1)
                .Select(g => new DashboardStatsDto(
                    g.Count(),
                    g.Count(c => c.Status == "Active"),
                    g.Count(c => !c.IsCompliant),
                    g.Average(c => c.AttendanceRate)
                ))
                .FirstOrDefaultAsync();

            // إرجاع قيم افتراضية في حال كانت الداتابيز فارغة
            return stats ?? new DashboardStatsDto(0, 0, 0, 0);
        }

        public async Task<List<MemberRiskDto>> GetOverloadedMembersAsync(int count)
        {
            return await _context.MemberWorkloads
                .AsNoTracking() // تحسين أداء للقراءة فقط
                .Where(m => m.TotalCommittees > 5 || m.PendingTasks > 10)
                .OrderByDescending(m => m.PendingTasks)
                .Take(count)
                .Select(m => new MemberRiskDto(m.MemberName, m.Department, m.TotalCommittees, m.PendingTasks))
                .ToListAsync();
        }

        public async Task<List<ComplianceReportDto>> GetNonCompliantCommitteesAsync()
        {
            return await _context.CommitteeSummaries
                .AsNoTracking()
                .Where(c => !c.IsCompliant)
                .Select(c => new ComplianceReportDto(c.Name, c.NonComplianceReason, c.LastActivityDate))
                .ToListAsync();
        }
    }
}
