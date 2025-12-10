using Audit.Application.DTOs;
using Audit.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Audit.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "AuditViewer")]
    public class AuditQueryController : ControllerBase
    {
        private readonly IAuditQueryService _service;

        public AuditQueryController(IAuditQueryService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> Query([FromQuery] AuditQueryParams query)
        {
            return Ok(await _service.QueryAsync(query));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            // this may require repo directly or Application service extension
            return Ok("Not implemented: implement GetById if needed");
        }
    }
}
