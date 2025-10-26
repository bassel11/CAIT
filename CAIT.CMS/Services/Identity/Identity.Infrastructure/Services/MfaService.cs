using Identity.Application.DTOs;
using Identity.Application.Interfaces;
using Identity.Core.Entities;
using Identity.Core.Enums;
using Microsoft.AspNetCore.Identity;
using OtpNet;
using System.Security.Cryptography;
using System.Text;

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

        public async Task<(bool Success, string? QrCodeUrl, string? Error)> EnableMfaAsync(string userId, MFAMethod method)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null || user.AuthType != ApplicationUser.AuthenticationType.Database)
                return (false, null, "User not found or invalid authentication type");

            // 🔒 أولاً: تعطيل أي إعدادات سابقة (تنظيف شامل)
            user.MFAEnabled = false;
            user.MFAMethod = MFAMethod.None;
            user.MFACode = null;
            user.MFACodeHash = null;
            user.MFACodeExpiry = null;
            user.AuthenticatorKey = null;

            string? qrCodeUri = null;

            // ⚙️ اختر الطريقة المطلوبة
            switch (method)
            {
                case MFAMethod.Email:
                    {
                        user.MFAEnabled = true;
                        user.MFAMethod = MFAMethod.Email;
                        user.MFACodeHash = null;
                        user.MFACodeExpiry = null;

                        await _userManager.UpdateAsync(user);

                        // 🔔 إخطار المستخدم
                        await _emailService.SendEmailAsync(
                            user.Email,
                            "MFA Enabled (Email Verification)",
                            "Multi-factor authentication via Email has been enabled on your account."
                        );

                        break;
                    }

                case MFAMethod.TOTP:
                    {
                        // توليد مفتاح سري ثابت (Base32)
                        var key = KeyGeneration.GenerateRandomKey(20);
                        user.AuthenticatorKey = Base32Encoding.ToString(key);
                        user.MFAEnabled = true;
                        user.MFAMethod = MFAMethod.TOTP;

                        await _userManager.UpdateAsync(user);

                        // إنشاء رابط QR لتطبيق المصادقة
                        string issuer = "CAIT Committee System";
                        qrCodeUri = $"otpauth://totp/{issuer}:{user.Email}?secret={user.AuthenticatorKey}&issuer={issuer}&digits=6";

                        break;
                    }

                case MFAMethod.None:
                    {
                        // تعطيل MFA بشكل كامل
                        await _userManager.UpdateAsync(user);

                        await _emailService.SendEmailAsync(
                            user.Email,
                            "MFA Disabled",
                            "Multi-factor authentication has been disabled on your account."
                        );

                        break;
                    }

                default:
                    return (false, null, "Unsupported MFA method. Allowed: None, Email, or TOTP.");
            }

            return (true, qrCodeUri, null);
        }

        public async Task<(bool Success, LoginResponseDto? Response, string? Error)> VerifyMfaAsync(VerifyMfaDto dto)
        {
            var user = await _userManager.FindByIdAsync(dto.UserId);
            if (user == null)
                return (false, null, "User not found");

            if (!user.MFAEnabled)
                return (false, null, "MFA not enabled");

            switch (user.MFAMethod)
            {
                case MFAMethod.Email:
                    if (string.IsNullOrEmpty(user.MFACodeHash) || !user.MFACodeExpiry.HasValue)
                        return (false, null, "MFA code not found");

                    if (user.MFACodeExpiry.Value < DateTime.UtcNow)
                    {
                        user.MFACodeHash = null;
                        user.MFACodeExpiry = null;
                        await _userManager.UpdateAsync(user);
                        return (false, null, "MFA code expired");
                    }

                    if (!VerifyCodeHash(dto.Code, user.MFACodeHash, user.SecurityStamp))
                        return (false, null, "Invalid MFA code");

                    // مسح الكود بعد النجاح
                    user.MFACodeHash = null;
                    user.MFACodeExpiry = null;
                    break;

                case MFAMethod.TOTP:
                    if (string.IsNullOrEmpty(user.AuthenticatorKey))
                        return (false, null, "Authenticator not configured");

                    var totp = new Totp(Base32Encoding.ToBytes(user.AuthenticatorKey));


                    if (!totp.VerifyTotp(dto.Code, out _, new VerificationWindow(1, 1)))
                        return (false, null, "Invalid or expired TOTP code");

                    // for 5 minutes use :
                    //bool valid = totp.VerifyTotp(dto.Code, out _, new VerificationWindow(previous: 10, future: 0));
                    //if (!valid)
                    //    return (false, null, "Invalid or expired TOTP code");

                    break;

                default:
                    return (false, null, "Unsupported MFA method");
            }

            await _userManager.UpdateAsync(user);

            // توليد JWT و Refresh Token بعد نجاح MFA
            var jwtResult = await _jwtTokenService.GenerateJwtTokenAsync(user);
            var refreshToken = await _refreshTokenService.GenerateRefreshTokenAsync(user);

            return (true, new LoginResponseDto
            {
                Token = jwtResult.Token,
                RefreshToken = refreshToken,
                TokenExpiry = jwtResult.Expiry
            }, null);
        }


        #region  Private Functions
        private string HashCode(string code, string salt)
        {
            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(salt));
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(code));
            return Convert.ToBase64String(hash);
        }

        private bool VerifyCodeHash(string code, string storedHash, string salt)
        {
            var newHash = HashCode(code, salt);
            return storedHash == newHash;
        }

        #endregion
    }
}
