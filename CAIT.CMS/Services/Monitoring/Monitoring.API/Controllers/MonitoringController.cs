using Asp.Versioning;
using BuildingBlocks.Shared.Controllers;
using BuildingBlocks.Shared.Wrappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Monitoring.Application.Dtos;
using Monitoring.Application.Features.Monitoring.Queries.GetComplianceReport;
using Monitoring.Application.Features.Monitoring.Queries.GetSuperAdminDashboard;

namespace Monitoring.API.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/Monitoring")]

    //[Route("api/[controller]")]
    //[ApiController]
    [Authorize]
    public class MonitoringController : BaseApiController
    {

        [HttpGet("dashboard/super-admin")]
        [Authorize(Policy = "Permission:Monitoring.View")]
        [ProducesResponseType(typeof(Result<SuperAdminDashboardResult>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetSuperAdminDashboard()
        {
            var result = await Mediator.Send(new GetSuperAdminDashboardQuery());

            return Success(result, "DashboardDataRetrievedSuccessfully");
        }

        // تقرير الامتثال
        [HttpGet("reports/compliance")]
        [Authorize(Policy = "Permission:Monitoring.View")]
        [ProducesResponseType(typeof(Result<List<ComplianceReportDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetComplianceReport()
        {
            var result = await Mediator.Send(new GetComplianceReportQuery());

            return Success(result, "ReportGeneratedSuccessfully");
        }

    }
}
