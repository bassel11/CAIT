using Identity.Application.DTOs;
using Identity.Application.Interfaces;
using Identity.Core.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Identity.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILdapAuthService _ldapAuthService;
        private readonly IMfaService _mfaService;

        public AuthController(IAuthService authService, ILdapAuthService ldapAuthService, IMfaService mfaService)
        {
            _authService = authService;
            _ldapAuthService = ldapAuthService;
            _mfaService = mfaService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            var result = await _authService.RegisterAsync(dto);
            if (!result.Success)
                return BadRequest(result.Errors);

            return Ok(result.Response);
        }

        // ------------------- Unified Login -------------------
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            (bool Success, LoginResponseDto? Response, string? Error, string? UserId) result;

            var authtype = (ApplicationUser.AuthenticationType)dto.AuthType;

            switch (authtype)
            {
                case ApplicationUser.AuthenticationType.OnPremAD:
                    result = await _ldapAuthService.LoginAsync(dto.Username, dto.Password);
                    break;

                case ApplicationUser.AuthenticationType.Database:
                    result = await _authService.LoginAsync(dto);
                    break;

                default:
                    return BadRequest("Unsupported authentication type");
            }

            if (!result.Success)
                return Unauthorized(result.Error);

            //return Ok(result.Response);
            return Ok(new
            {
                result.Success,
                result.Response,
                result.Error,
                result.UserId
            });
        }

        // ------------------- Unified Refresh Token -------------------
        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequestDto dto)
        {
            // نفترض أن نوع المصادقة يتم إرساله في DTO أيضًا
            // يمكنك تعديله حسب حاجتك
            if (dto.AuthType == (int)ApplicationUser.AuthenticationType.OnPremAD)
            {
                var ldapResult = await _ldapAuthService.RefreshTokenAsync(dto.Token, dto.RefreshToken);
                if (!ldapResult.Success)
                    return Unauthorized(ldapResult.Error);
                return Ok(ldapResult.Response);
            }
            else if (dto.AuthType == (int)ApplicationUser.AuthenticationType.Database)
            {
                var dbResult = await _authService.RefreshTokenAsync(dto.Token, dto.RefreshToken);
                if (!dbResult.Success)
                    return Unauthorized(dbResult.Error);
                return Ok(dbResult.Response);
            }

            return BadRequest("Unsupported authentication type");
        }

        [HttpPost("verify-mfa")]
        public async Task<IActionResult> VerifyMfa(VerifyMfaDto dto)
        {
            var result = await _mfaService.VerifyMfaAsync(dto);

            if (!result.Success)
                return Unauthorized(result.Error);

            return Ok(result.Response);
        }
    }
}
