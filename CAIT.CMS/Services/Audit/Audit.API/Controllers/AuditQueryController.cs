using Audit.Application.DTOs;
using Audit.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Audit.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "Permission:Audit.View")]
    public class AuditQueryController : ControllerBase
    {
        private readonly IAuditQueryService _service;
        private readonly IAuditQueryNewService _queryService;

        public AuditQueryController(IAuditQueryService service, IAuditQueryNewService queryService)
        {
            _service = service;
            _queryService = queryService;
        }

        [HttpGet]
        public async Task<IActionResult> Query([FromQuery] AuditQueryParams query)
        {
            return Ok(await _service.QueryAsync(query));
        }

        [HttpGet("history")]
        public async Task<IActionResult> GetHistory([FromQuery] string entity, [FromQuery] string id)
        {
            if (string.IsNullOrEmpty(entity) || string.IsNullOrEmpty(id))
                return BadRequest("Entity Name and ID are required.");

            var history = await _queryService.GetHistoryAsync(entity, id);

            return Ok(history);
        }

        // نقطة نهاية للتقارير الشاملة [المصدر: 31-33]
        [HttpGet("report")]
        public async Task<IActionResult> GetReport([FromQuery] string? committeeId, [FromQuery] DateTime? from, [FromQuery] DateTime? to)
        {
            // هنا يمكنك إضافة منطق تقارير PDF مستقبلاً
            return Ok("Report generation endpoint ready.");
        }

    }
}
