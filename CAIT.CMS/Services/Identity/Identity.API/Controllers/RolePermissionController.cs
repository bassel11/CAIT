using Identity.Application.DTOs.RolePermissions;
using Identity.Application.Interfaces.RolePermissions;
using Microsoft.AspNetCore.Mvc;

namespace Identity.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RolePermissionController : BaseController
    {
        private readonly IRolePermissionService _rolePermission;
        public RolePermissionController(IRolePermissionService rolePermission)
        {
            _rolePermission = rolePermission;
        }

        [HttpPost("assign")]
        public async Task<IActionResult> AssignPermissionsToRole([FromBody] AssignPermissionsToRoleDto dto)
        {
            var result = await _rolePermission.AssignPermissionsToRoleAsync(dto);
            return Ok(result);
        }

        [HttpGet("role/{roleId}")]
        public async Task<IActionResult> GetPermissionsByRole(Guid roleId, [FromQuery] PermissionByRoleFilterDto filter)
        {
            var result = await _rolePermission.GetPermissionsByRoleAsync(roleId, filter);
            return Ok(result);
        }
    }
}
