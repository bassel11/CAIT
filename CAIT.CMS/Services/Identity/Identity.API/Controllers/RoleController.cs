using Identity.Application.DTOs.Roles;
using Identity.Application.Interfaces.Roles;
using Microsoft.AspNetCore.Mvc;

namespace Identity.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(Roles = "SuperAdmin,Admin")]
    public class RoleController : ControllerBase
    {
        private readonly IRoleService _roleService;

        public RoleController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        [HttpGet("GetRoles")]
        public async Task<IActionResult> GetRoles([FromQuery] RoleFilterDto filter)
            => Ok(await _roleService.GetRolesAsync(filter));

        [HttpGet("GetRoleById{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var role = await _roleService.GetByIdAsync(id);
            return role == null ? NotFound() : Ok(role);
        }

        [HttpPost("CreateRole")]
        public async Task<IActionResult> Create([FromBody] RoleCreateDto dto)
        {
            var result = await _roleService.CreateAsync(dto);
            return result.Success ? Ok() : BadRequest(result.Error);
        }

        [HttpPut("UpdateRole{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] RoleUpdateDto dto)
        {
            var result = await _roleService.UpdateAsync(id, dto);
            return result.Success ? Ok() : BadRequest(result.Error);
        }

        [HttpDelete("DeleteRole{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _roleService.DeleteAsync(id);
            return result.Success ? Ok() : NotFound(result.Error);
        }
    }
}
