using Identity.API.Controllers.Base;
using Identity.Application.DTOs.RolePermissions;
using Identity.Application.Interfaces.RolePermissions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Identity.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RolePermissionController : BaseController
    {
        private readonly IRolePremissionService _rolePermission;
        public RolePermissionController(IRolePremissionService rolePermission)
        {
            _rolePermission = rolePermission;
        }

        [HttpPost("AssignPermissionsToRole")]
        [Authorize(Policy = "Permission:RolePermission.Assign")]
        public async Task<IActionResult> AssignPermissionsToRole([FromBody] AsgnPermsToRoleDto dto)
        {
            var result = await _rolePermission.AsgnPermsToRoleAsync(dto);
            return Ok(result);
        }

        [HttpGet("GetPermissionsByRole")]
        [Authorize(Policy = "Permission:RolePermission.View")]
        public async Task<IActionResult> GetPermissionsByRole([FromQuery] PermsByRoleFilterDto filter)
        {
            var result = await _rolePermission.GetPermsByRoleAsync(filter);
            return Ok(result);
        }

        [HttpPut("RemovePermissionsOfRole")]
        [Authorize(Policy = "Permission:RolePermission.Remove")]
        public async Task<IActionResult> RemovePermissionsOfRoleAsync([FromBody] RemPermsToRoleDto dto)
        {
            var result = await _rolePermission.RemovePermissionsOfRoleAsync(dto);
            return Ok(result);
        }
    }
}
