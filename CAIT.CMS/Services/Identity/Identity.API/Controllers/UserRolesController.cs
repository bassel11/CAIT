using Identity.Application.DTOs.UserRoles;
using Identity.Application.DTOs.UserRoles.Multiple;
using Identity.Application.Interfaces.UserRoles;
using Microsoft.AspNetCore.Mvc;

namespace Identity.API.Controllers
{
    public class UserRolesController : BaseController
    {
        private readonly IUserRoleService _userroleService;

        public UserRolesController(IUserRoleService userroleService)
        {
            _userroleService = userroleService;
        }

        [HttpPost("assign")]
        public async Task<IActionResult> AssigUserRole([FromBody] AssignUserRoleDto dto)
        {
            var success = await _userroleService.AssignUserRoleAsync(dto.UserId, dto.RoleName);
            return success ? Ok(new { Message = $"Role '{dto.RoleName}' assigned to user" })
                           : BadRequest(new { Message = $"Failed to assign role '{dto.RoleName}'" });
        }

        [HttpPost("remove")]
        public async Task<IActionResult> RemoveUserRole([FromBody] RemoveUserRoleDto dto)
        {
            var success = await _userroleService.RemoveUserRoleAsync(dto.UserId, dto.RoleName);
            return success ? Ok(new { Message = $"Role '{dto.RoleName}' removed from user" })
                           : BadRequest(new { Message = $"Failed to remove role '{dto.RoleName}'" });
        }

        [HttpGet("roles/{userId}")]
        public async Task<IActionResult> GetUserRoles(Guid userId)
        {
            var roles = await _userroleService.GetUserRolesAsync(userId);
            return Ok(new UserRolesDto
            {
                UserId = userId,
                Email = User.Identity?.Name ?? "",
                Roles = roles
            });
        }

        [HttpGet("users/{role}")]
        public async Task<IActionResult> GetUsersByRole(string role)
        {
            var users = await _userroleService.GetUsersByRoleAsync(role);
            var result = new RoleUsersDto
            {
                RoleName = role,
                Users = users.Select(u => new UserRolesDto
                {
                    UserId = u.Id,
                    Email = u.Email,
                    Roles = _userroleService.GetUserRolesAsync(u.Id).Result
                })
            };

            return Ok(result);
        }

        #region Multiple 
        [HttpPost("assign-multiple")]
        public async Task<IActionResult> AssignUserRoles([FromBody] AssignUserRolesDto dto)
        {
            var (success, message) = await _userroleService.AssignUserRolesAsync(dto.UserId, dto.RoleNames);
            return success ? Ok(new { Message = message })
                   : BadRequest(new { Message = message });
        }

        [HttpPost("remove-multiple")]
        public async Task<IActionResult> RemoveUserRoles([FromBody] RemoveUserRolesDto dto)
        {
            var success = await _userroleService.RemoveUserRolesAsync(dto.UserId, dto.RoleNames);
            return success ? Ok(new { Message = "Roles removed successfully" })
                           : BadRequest(new { Message = "Failed to remove roles" });
        }

        [HttpPost("assign-users-to-role")]
        public async Task<IActionResult> AssignUsersToRole([FromBody] AssignUsersToRoleDto dto)
        {
            var success = await _userroleService.AssignUsersToRoleAsync(dto.RoleName, dto.UserIds);
            return success ? Ok(new { Message = "Users assigned to role successfully" })
                           : BadRequest(new { Message = "Failed to assign users" });
        }

        #endregion



    }
}
