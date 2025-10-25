using Identity.Application.DTOs.Users;
using Identity.Application.Interfaces.Users;
using Microsoft.AspNetCore.Mvc;

namespace Identity.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(Roles = "Admin,SuperAdmin")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService) => _userService = userService;

        [HttpGet("GetUsers")]
        public async Task<IActionResult> GetAll([FromQuery] UserFilterDto filter)
        => Ok(await _userService.GetUsersAsync(filter));

        [HttpGet("GetUserById/{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var user = await _userService.GetByIdAsync(id);
            return user == null ? NotFound() : Ok(user);
        }

        [HttpPut("UpdateUser/{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UserUpdateDto dto)
        {
            var result = await _userService.UpdateAsync(id, dto);
            return result.Success ? Ok() : BadRequest(result.Error);
        }

        [HttpDelete("DeleteUser{id:guid}")]
        public async Task<IActionResult> SoftDelete(Guid id)
        {
            var result = await _userService.SoftDeleteAsync(id);
            return result.Success ? Ok() : NotFound(result.Error);
        }

    }
}
