using Identity.API.Models;
using Identity.Application.DTOs;
using Identity.Application.Interfaces;
using Identity.Core.Entities;
using Identity.Core.Enums;
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
        private readonly IGuestUserAsync _guestUser;

        public AuthController(IAuthService authService, ILdapAuthService ldapAuthService, IMfaService mfaService, IAzureAuthService azureAuthService, IAzureB2BService azureB2BService, IGuestUserAsync guestUser)
        {
            _authService = authService;
            _ldapAuthService = ldapAuthService;
            _mfaService = mfaService;
            _azureAuthService = azureAuthService;
            _azureB2BService = azureB2BService;
            _guestUser = guestUser;
        }

        // register
        [HttpPost("register")]
        [Authorize(Roles = "SuperAdmin", Policy = "RegisterUser", AuthenticationSchemes = "BearerPolicy")]
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

            string? token = null;

            // استخراج Authorization Header فقط إذا كان النوع AzureAD أو B2BGuest
            if (authtype == ApplicationUser.AuthenticationType.AzureAD || authtype == ApplicationUser.AuthenticationType.B2BGuest)
            {
                var authHeader = Request.Headers["Authorization"].ToString();
                if (string.IsNullOrWhiteSpace(authHeader) || !authHeader.StartsWith("Bearer "))
                    return BadRequest("Missing or invalid Authorization header");

                token = authHeader.Substring("Bearer ".Length).Trim();
            }

            switch (authtype)
            {
                case ApplicationUser.AuthenticationType.OnPremAD:
                    result = await _ldapAuthService.LoginAsync(dto.Username, dto.Password);
                    break;

                case ApplicationUser.AuthenticationType.Database:
                    result = await _authService.LoginAsync(dto);
                    break;

                case ApplicationUser.AuthenticationType.AzureAD:

                    if (await _guestUser.IsGuestUserAsync(token!))
                        return Unauthorized("Guest users must login via B2BGuest flow");

                    result = await _azureAuthService.ExchangeAzureTokenAsync(token!);
                    break;

                case ApplicationUser.AuthenticationType.B2BGuest:

                    if (!await _guestUser.IsGuestUserAsync(token!))
                        return Unauthorized("Member users must login via AzureAD flow");
                    result = await _azureB2BService.ExchangeB2BTokenAsync(token);

                    break;

                default:
                    return BadRequest("Unsupported authentication type");
            }

            if (!result.Success)
            {
                return Unauthorized(new ErrorResponse
                {
                    StatusCode = StatusCodes.Status401Unauthorized,
                    Error = "Unauthorized",
                    Message = result.Error ?? "Login failed due to invalid credentials or authentication error."
                });

            }

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


        // verify-mfa
        [HttpPost("verify-mfa")]

        public async Task<IActionResult> VerifyMfa(VerifyMfaDto dto)
        {
            var result = await _mfaService.VerifyMfaAsync(dto);

            if (!result.Success)
                return Unauthorized(result.Error);

            return Ok(result.Response);
        }

        // enable mfa
        [HttpPost("enable-mfa")]
        [Authorize(Roles = "SuperAdmin", Policy = "EnableMfa", AuthenticationSchemes = "BearerPolicy")]
        public async Task<IActionResult> EnableMfaForDatabaseUser([FromBody] EnableMfaDto dto)
        {
            if (string.IsNullOrEmpty(dto.UserId))
                return BadRequest("UserId is required");

            // 🔹 تحقق من أن طريقة الـ MFA صالحة
            if (!Enum.IsDefined(typeof(MFAMethod), dto.DeliveryMethod))
                return BadRequest("Invalid DeliveryMethod. Allowed values: None, Email, TOTP.");

            var result = await _mfaService.EnableMfaAsync(dto.UserId, dto.DeliveryMethod);
            if (!result.Success)
                return BadRequest(result.Error);

            // إرجاع الاستجابة مع QR فقط إذا كانت TOTP
            if (dto.DeliveryMethod == MFAMethod.TOTP)
            {
                return Ok(new
                {
                    Message = $"{dto.DeliveryMethod}-based MFA enabled successfully for user {dto.UserId}",
                    QrCodeUri = result.QrCodeUrl
                });
            }
            else
            {
                return Ok(new
                {
                    Message = $"{dto.DeliveryMethod}-based MFA {(dto.DeliveryMethod == MFAMethod.None ? "disabled" : "enabled")} successfully for user {dto.UserId}"
                });
            }

        }



        // change password
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


        //
        [Authorize(Policy = "CreateCommittee", AuthenticationSchemes = "BearerPolicy")]
        [HttpPost("committee/create")]
        public IActionResult CreateMeeting() //Guid committeeId, [FromBody] CreateMeetingDto dto
        {
            // authorized users only
            return Ok("hellloooooo");
        }
    }
}
