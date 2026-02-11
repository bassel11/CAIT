using Asp.Versioning;
using Audit.Application.DTOs;
using Audit.Application.Services;
using BuildingBlocks.Shared.Controllers;
using BuildingBlocks.Shared.Wrappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Audit.API.Controllers
{

    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/Audit")]
    [Authorize(Policy = "Permission:Audit.View")]
    public class AuditQueryController : BaseApiController
    {
        private readonly IAuditQueryService _service;
        private readonly IAuditQueryNewService _queryService;

        public AuditQueryController(IAuditQueryService service, IAuditQueryNewService queryService)
        {
            _service = service;
            _queryService = queryService;
        }

        // -------------------------------------------------------
        // GET Query (Filter Logs)
        // -------------------------------------------------------
        [HttpGet]
        [ProducesResponseType(typeof(Result<object>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Query([FromQuery] AuditQueryParams query)
        {
            var data = await _service.QueryAsync(query);
            return Success(data);
        }

        // -------------------------------------------------------
        // GET History (By Entity & ID)
        // -------------------------------------------------------
        [HttpGet("history")]
        [ProducesResponseType(typeof(Result<IEnumerable<object>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result<string>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetHistory([FromQuery] string entity, [FromQuery] string id)
        {
            if (string.IsNullOrEmpty(entity) || string.IsNullOrEmpty(id))
            {
                return BadRequest(Result<string>.Failure("Entity Name and ID are required."));
            }

            var history = await _queryService.GetHistoryAsync(entity, id);

            return Success(history);
        }

        // -------------------------------------------------------
        // GET Report (Placeholder)
        // -------------------------------------------------------
        [HttpGet("report")]
        [ProducesResponseType(typeof(Result<string>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetReport(
            [FromQuery] string? committeeId,
            [FromQuery] DateTime? from,
            [FromQuery] DateTime? to)
        {
            return Success("Report generation endpoint ready.");
        }

    }
}
