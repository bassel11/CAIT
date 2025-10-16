using Identity.Application.DTOs;
using Identity.Application.Interfaces;
using Identity.Core.Entities;
using Microsoft.AspNetCore.Identity;

namespace Identity.Infrastructure.Services
{
    public class MfaService : IMfaService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailService _emailService;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly IRefreshTokenService _refreshTokenService;

        public MfaService(
            UserManager<ApplicationUser> userManager,
            IEmailService emailService,
            IJwtTokenService jwtTokenService,
            IRefreshTokenService refreshTokenService)
        {
            _userManager = userManager;
            _emailService = emailService;
            _jwtTokenService = jwtTokenService;
            _refreshTokenService = refreshTokenService;
        }
        public async Task<(bool Success, LoginResponseDto? Response, string? Error)> VerifyMfaAsync(VerifyMfaDto dto)
        {
            var user = await _userManager.FindByIdAsync(dto.UserId);
            if (user == null)
                return (false, null, "User not found");

            if (!user.MFAEnabled || string.IsNullOrEmpty(user.MFACode) || !user.MFACodeExpiry.HasValue)
                return (false, null, "MFA not enabled or code invalid");

            if (user.MFACodeExpiry < DateTime.UtcNow)
                return (false, null, "MFA code expired");

            if (user.MFACode != dto.Code)
                return (false, null, "Invalid MFA code");

            // مسح الكود بعد التحقق
            user.MFACode = null;
            user.MFACodeExpiry = null;
            await _userManager.UpdateAsync(user);

            // توليد JWT و Refresh Token
            var token = _jwtTokenService.GenerateJwtToken(user, out var expiry);
            var refreshToken = await _refreshTokenService.GenerateRefreshTokenAsync(user);

            return (true, new LoginResponseDto
            {
                Token = token,
                RefreshToken = refreshToken,
                TokenExpiry = expiry
            }, null);
        }
    }
}
