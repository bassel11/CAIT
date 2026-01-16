using Identity.API.Controllers.Base;
using Identity.Application.DTOs.Pre.Custom;
using Identity.Application.Interfaces.UsrRolPermRes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Identity.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsrRolPermResController : BaseController
    {
        #region Fields

        private readonly IUsrRolPermResService _usrRolPermResService;

        #endregion

        #region Constructors
        public UsrRolPermResController(IUsrRolPermResService usrRolPermResService)
        {
            _usrRolPermResService = usrRolPermResService;
        }

        #endregion

        #region Actions

        [HttpPost("AssignCustomPermissions")]
        [Authorize(Policy = "Permission:CustomPermission.Assign")]
        public async Task<IActionResult> AssignCustomPermissions([FromBody] AssignCustomPermsDto dto)
        {
            var result = await _usrRolPermResService.AssignCustomPermsAsync(dto);
            if (!result)
                return BadRequest("User cannot be assigned custom permissions (may have roles or invalid PrivilageType).");

            return Ok("Custom permissions assigned successfully.");
        }

        // Get Custom Permissions
        [HttpGet("GetCustomPermissions/{userId}")]
        [Authorize(Policy = "Permission:CustomPermission.View")]
        public async Task<IActionResult> GetCustomPermissions(
            [FromRoute] Guid userId,
            [FromQuery] CustomPermFilterDto? filter = null)
        {
            var perms = await _usrRolPermResService.GetCustomPermsAsync(userId, filter);
            return Ok(perms);
        }


        // Remove Custom Permission

        [HttpDelete("RemoveCustomPermissions")]
        [Authorize(Policy = "Permission:CustomPermission.Remove")]
        public async Task<IActionResult> RemoveCustomPermission([FromBody] RemoveCustomPermsDto dto)
        {
            var result = await _usrRolPermResService.RemoveCustomPermsAsync(dto);
            if (!result)
                return BadRequest("Failed to remove custom permission.");

            return Ok("Custom permission removed successfully.");
        }

        // Check if User has Custom Permissions
        [HttpGet("HasCustomPermissions/{userId}")]
        [Authorize(Policy = "Permission:CustomPermission.View")]
        public async Task<IActionResult> HasCustomPermissions([FromRoute] Guid userId)
        {
            var hasPerms = await _usrRolPermResService.HasCustomPermissionsAsync(userId);
            return Ok(new { UserId = userId, HasCustomPermissions = hasPerms });
        }

        #endregion


    }
}
