using Identity.Application.DTOs;
using Identity.Application.Interfaces;
using Identity.Core.Entities;
using Microsoft.AspNetCore.Authorization;
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
        private readonly IAzureAuthService _azureAuthService;
        private readonly IAzureB2BService _azureB2BService;

        public AuthController(IAuthService authService, ILdapAuthService ldapAuthService, IMfaService mfaService, IAzureAuthService azureAuthService, IAzureB2BService azureB2BService)
        {
            _authService = authService;
            _ldapAuthService = ldapAuthService;
            _mfaService = mfaService;
            _azureAuthService = azureAuthService;
            _azureB2BService = azureB2BService;
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

                case ApplicationUser.AuthenticationType.AzureAD:

                    var authADHeader = Request.Headers["Authorization"].ToString();
                    if (string.IsNullOrWhiteSpace(authADHeader) || !authADHeader.StartsWith("Bearer "))
                        return BadRequest("Missing or invalid Authorization header");

                    var Azuretoken = authADHeader.Substring("Bearer ".Length).Trim();
                    result = await _azureAuthService.ExchangeAzureTokenAsync(Azuretoken);
                    break;

                case ApplicationUser.AuthenticationType.B2BGuest:

                    var authB2BHeader = Request.Headers["Authorization"].ToString();
                    if (string.IsNullOrWhiteSpace(authB2BHeader) || !authB2BHeader.StartsWith("Bearer "))
                        return BadRequest("Missing or invalid Authorization header");

                    var B2Btoken = authB2BHeader.Substring("Bearer ".Length).Trim();
                    result = await _azureB2BService.ExchangeB2BTokenAsync(B2Btoken);

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


        [HttpPost("change-password")]
        [Authorize(AuthenticationSchemes = "BearerPolicy")]
        public async Task<IActionResult> ChangePassword(ChangePasswordDto dto)
        {
            var userId = User.FindFirst("uid")?.Value;   // sub
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("Invalid user context");

            var result = await _authService.ChangePasswordAsync(userId, dto.CurrentPassword, dto.NewPassword);
            if (!result.Success)
                return BadRequest(result.Error);

            return Ok(new { Message = "Password changed successfully" });
        }


        [HttpPost("enable-mfa")]
        [Authorize(AuthenticationSchemes = "BearerPolicy")]
        public async Task<IActionResult> EnableMfaForDatabaseUser(EnableMfaDto dto)
        {
            if (string.IsNullOrEmpty(dto.UserId))
                return BadRequest("UserId is required");

            var result = await _mfaService.EnableMfaAsync(dto.UserId, "Email"); // أو طريقة أخرى حسب النظام
            if (!result.Success)
                return BadRequest(result.Error);

            return Ok(new { Message = $"MFA enabled successfully for user {dto.UserId}" });
        }


    }
}
