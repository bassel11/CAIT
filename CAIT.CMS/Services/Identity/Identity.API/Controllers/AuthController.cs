using Identity.Application.DTOs;
using Identity.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Identity.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILdapAuthService _ldapAuthService;

        public AuthController(IAuthService authService, ILdapAuthService ldapAuthService)
        {
            _authService = authService;
            _ldapAuthService = ldapAuthService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            var result = await _authService.RegisterAsync(dto);
            if (!result.Success) return BadRequest(result.Errors);
            return Ok(result.Response);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var result = await _authService.LoginAsync(dto);
            if (!result.Success) return Unauthorized(result.Error);
            return Ok(result.Response);
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequestDto dto)
        {
            var result = await _authService.RefreshTokenAsync(dto.Token, dto.RefreshToken);
            if (!result.Success) return Unauthorized(result.Error);
            return Ok(result.Response);
        }

        [HttpPost("login-ldap")]
        public async Task<IActionResult> LoginLdap(LdapLoginDto dto)
        {
            // استدعاء خدمة LDAP للتحقق من المستخدم
            var ldapResult = await _ldapAuthService.AuthenticateAsync(dto.Username, dto.Password);

            if (!ldapResult.Success)
                return Unauthorized(ldapResult.Error);

            // بناء الرد
            var response = new LdapLoginResponseDto
            {
                Success = true,
                ExternalId = ldapResult.ExternalId
            };

            return Ok(response);
        }

    }
}
