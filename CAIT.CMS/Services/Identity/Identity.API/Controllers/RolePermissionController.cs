using Identity.API.Controllers.Base;
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

        [HttpPost("AssignPermissionsToRole")]
        public async Task<IActionResult> AssignPermissionsToRole([FromBody] AssignPermissionsToRoleDto dto)
        {
            var result = await _rolePermission.AssignPermissionsToRoleAsync(dto);
            return Ok(result);
        }

        [HttpGet("GetPermissionsByRole")]
        public async Task<IActionResult> GetPermissionsByRole([FromQuery] PermissionByRoleFilterDto filter)
        {
            var result = await _rolePermission.GetPermissionsByRoleAsync(filter);
            return Ok(result);
        }

        [HttpGet("GetRolePermissions")]
        public async Task<IActionResult> GetRolePermissions([FromQuery] PermissionByRoleFilterDto filter)
        {
            var permissions = await _rolePermission.GetRolePermissionsWithResourcesAsync(filter);
            return Ok(permissions);
        }
    }
}
