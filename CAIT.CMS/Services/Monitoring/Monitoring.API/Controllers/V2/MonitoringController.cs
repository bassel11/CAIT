using Asp.Versioning;
using BuildingBlocks.Shared.Controllers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Monitoring.Application.Features.Monitoring.Queries.GetComplianceReport;
using Monitoring.Application.Features.Monitoring.Queries.GetSuperAdminDashboard;

namespace Monitoring.API.Controllers.V2
{
    [ApiVersion("2.0")]
    [Route("api/v{version:apiVersion}/Monitoring")]

    //[Route("api/[controller]")]
    //[ApiController]
    [Authorize]
    public class MonitoringController : BaseApiController
    {
        private readonly IMediator _mediator;

        public MonitoringController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // لوحة قيادة المدير العام (Super Admin Dashboard)
        [HttpGet("dashboard/super-admin")]
        [Authorize(Policy = "Permission:Monitoring.View")]
        public async Task<IActionResult> GetSuperAdminDashboard()
        {
            // إرسال الطلب (Query) واستلام النتيجة
            var result = await _mediator.Send(new GetSuperAdminDashboardQuery());
            return Ok(result);
        }

        // تقرير الامتثال
        [HttpGet("reports/compliance")]
        [Authorize(Policy = "Permission:Monitoring.View")]
        public async Task<IActionResult> GetComplianceReport()
        {
            var result = await _mediator.Send(new GetComplianceReportQuery());
            return Ok(result);
        }
    }

}
