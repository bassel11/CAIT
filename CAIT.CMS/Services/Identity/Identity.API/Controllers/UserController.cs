using Identity.API.Controllers.Base;
using Identity.Application.DTOs;
using Identity.Application.DTOs.Users;
using Identity.Application.Interfaces.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Identity.API.Controllers
{
    public class UserController : BaseController
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService) => _userService = userService;

        [HttpGet("GetUsers")]
        [Authorize(Policy = "Permission:User.View")]
        public async Task<IActionResult> GetAll([FromQuery] UserFilterDto filter)
        => Ok(await _userService.GetUsersAsync(filter));

        [HttpGet("GetUserById/{id:guid}")]
        [Authorize(Policy = "Permission:User.View")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var user = await _userService.GetByIdAsync(id);
            return user == null ? NotFound() : Ok(user);
        }

        [HttpPut("UpdateUser/{id:guid}")]
        [Authorize(Policy = "Permission:User.Update")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UserUpdateDto dto)
        {
            var result = await _userService.UpdateAsync(id, dto);
            return result.Success ? Ok() : BadRequest(result.Error);
        }

        [HttpDelete("DeleteUser{id:guid}")]
        [Authorize(Policy = "Permission:User.Delete")]
        public async Task<IActionResult> SoftDelete(Guid id)
        {
            var result = await _userService.SoftDeleteAsync(id);
            return result.Success ? Ok() : NotFound(result.Error);
        }

        // Dectivate User Only for SuperAdmin Roles
        [HttpPost("deactivateUser")]
        [Authorize(Policy = "Permission:User.Deactivate")]
        public async Task<IActionResult> DeactivateUser(DeactivateUserDto dto)
        {

            if (string.IsNullOrEmpty(dto.UserId))
                return Unauthorized("Invalid user");

            var result = await _userService.DeactivateUserAsync(dto.UserId);
            if (!result.Success)
                return BadRequest(result.Error);

            return Ok(new { Message = "Deactivate User successfully" });
        }

        // Activate User Only for SuperAdmin Roles (or authorized users)
        [HttpPost("activateUser")]
        [Authorize(Policy = "Permission:User.Activate")] // ✅ تأكد من إضافة هذه الصلاحية في قاعدة البيانات
        public async Task<IActionResult> ActivateUser([FromBody] ActivateUserDto dto)
        {
            // التحقق من صحة المدخلات
            if (string.IsNullOrEmpty(dto.UserId))
                return BadRequest("Invalid User ID");

            // استدعاء الخدمة
            var result = await _userService.ActivateUserAsync(dto.UserId);

            // التعامل مع الأخطاء
            if (!result.Success)
            {
                // يمكن تخصيص الرد أكثر إذا كان الخطأ "User not found" ليكون NotFound
                if (result.Error == "User not found")
                    return NotFound(result.Error);

                return BadRequest(result.Error);
            }

            return Ok(new { Message = "User activated successfully" });
        }


    }
}
