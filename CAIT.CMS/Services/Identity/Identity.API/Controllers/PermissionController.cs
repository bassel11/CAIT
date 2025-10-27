using Identity.Application.DTOs.Permissions;
using Identity.Application.Interfaces.Permissions;
using Microsoft.AspNetCore.Mvc;

namespace Identity.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PermissionController : BaseController
    {
        private readonly IPermissionService _permissionService;

        public PermissionController(IPermissionService permissionService)
        {
            _permissionService = permissionService;
        }

        [HttpGet("GetAllPermissions")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _permissionService.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("GetPermissionById/{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _permissionService.GetByIdAsync(id);
            return Ok(result);
        }

        [HttpPost("CreatePermission")]
        public async Task<IActionResult> Create([FromBody] CreatePermissionDto dto)
        {
            var result = await _permissionService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpPut("UpdatePermission/{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdatePermissionDto dto)
        {
            var result = await _permissionService.UpdateAsync(id, dto);
            return Ok(result);
        }

        [HttpDelete("DeletePermission/{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _permissionService.DeleteAsync(id);
            return Ok(result);
        }

    }
}
