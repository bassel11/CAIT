//using Microsoft.AspNetCore.Mvc;

//namespace Monitoring.API.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class MonitoringController : ControllerBase
//    {
//        private readonly AnalyticsDbContext _context;

//        public AnalyticsController(AnalyticsDbContext context)
//        {
//            _context = context;
//        }

//        // لوحة قيادة المدير العام (Super Admin Dashboard)
//        [HttpGet("dashboard/super-admin")]
//        public async Task<IActionResult> GetSuperAdminDashboard()
//        {
//            var stats = await _context.CommitteeSummaries
//                .GroupBy(c => 1)
//                .Select(g => new
//                {
//                    TotalCommittees = g.Count(),
//                    ActiveCount = g.Count(c => c.Status == "Active"),
//                    NonCompliantCount = g.Count(c => !c.IsCompliant),
//                    AvgAttendance = g.Average(c => c.AttendanceRate)
//                })
//                .FirstOrDefaultAsync();

//            var overloadedMembers = await _context.MemberWorkloads
//                .Where(m => m.TotalCommittees > 5 || m.PendingTasks > 10)
//                .Take(10)
//                .ToListAsync();

//            return Ok(new { Stats = stats, RiskList = overloadedMembers });
//        }

//        // تقرير الامتثال
//        [HttpGet("reports/compliance")]
//        public async Task<IActionResult> GetComplianceReport()
//        {
//            var risks = await _context.CommitteeSummaries
//                .Where(c => !c.IsCompliant)
//                .Select(c => new { c.Name, c.NonComplianceReason, c.LastActivityDate })
//                .ToListAsync();

//            return Ok(risks);
//        }
//    }
//}
