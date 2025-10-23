using Identity.Application.DTOs;
using Identity.Application.Interfaces;
using Identity.Core.Entities;
using Identity.Core.Enums;
using Identity.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using static Identity.Core.Entities.ApplicationUser;

namespace Identity.Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _dbContext;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly IRefreshTokenService _refreshTokenService;
        private readonly IEmailService _emailService;
        public AuthService(UserManager<ApplicationUser> userManager, ApplicationDbContext dbContext, IJwtTokenService jwtTokenService,
            IRefreshTokenService refreshTokenService, IEmailService emailService)
        {
            _userManager = userManager;
            _dbContext = dbContext;
            _jwtTokenService = jwtTokenService;
            _refreshTokenService = refreshTokenService;
            _emailService = emailService;
        }

        public async Task<(bool Success, LoginResponseDto? Response, IEnumerable<string>? Errors)> RegisterAsync(RegisterDto dto)
        {
            var user = new ApplicationUser
            {
                UserName = dto.Email,
                Email = dto.Email,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                AuthType = AuthenticationType.Database,
                MFAEnabled = false,
                EmailConfirmed = true,
                IsActive = true
            };

            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded) return (false, null, result.Errors.Select(e => e.Description));

            // Assign default role
            await _userManager.AddToRoleAsync(user, "Member");

            // 🟢 حفظ كلمة المرور الجديدة في سجل UserPasswordHistory
            var passwordHistory = new UserPasswordHistory
            {
                UserId = user.Id,
                PasswordHash = user.PasswordHash!,   // تم توليدها تلقائياً من Identity
                CreatedAt = DateTime.UtcNow
            };
            _dbContext.UserPasswordHistories.Add(passwordHistory);
            await _dbContext.SaveChangesAsync();

            var token = _jwtTokenService.GenerateJwtToken(user, out var expiry);
            var refreshToken = await _refreshTokenService.GenerateRefreshTokenAsync(user);

            var response = new LoginResponseDto
            {
                Token = token,
                RefreshToken = refreshToken,
                TokenExpiry = expiry
            };

            return (true, response, null);
        }

        public async Task<(bool Success, LoginResponseDto? Response, string? Error, string? UserId)> LoginAsync(LoginDto dto)
        {
            // البحث عن المستخدم مع مراعاة نوع المصادقة Database فقط
            var user = await _userManager.Users
                .FirstOrDefaultAsync(u => u.Email == dto.Email && u.AuthType == ApplicationUser.AuthenticationType.Database);

            if (user == null || !await _userManager.CheckPasswordAsync(user, dto.Password))
                return (false, null, "Invalid credentials", null);

            if (!user.IsActive)
                return (false, null, "User is inactive", null);

            if (user.ExpirationDate.HasValue && user.ExpirationDate < DateTime.UtcNow)
                return (false, null, "Account expired", null);

            // التحقق من MFA
            if (user.MFAEnabled)
            {
                switch (user.MFAMethod)
                {
                    case MFAMethod.Email:
                        // توليد كود مؤقت وإرساله عبر البريد
                        var code = GenerateNumericCode(6);
                        user.MFACodeHash = HashCode(code, user.SecurityStamp);
                        user.MFACodeExpiry = DateTime.UtcNow.AddMinutes(5);
                        await _userManager.UpdateAsync(user);

                        await _emailService.SendMfaCodeAsync(user.Email, code);
                        break;

                    case MFAMethod.TOTP:
                        // TOTP يعتمد على Authenticator app، لا حاجة لإنشاء كود جديد
                        break;

                    default:
                        return (false, null, "Unsupported MFA method", user.Id.ToString());
                }

                // العودة لتخبر العميل أن MFA مطلوب
                return (true, null, "MFARequired", user.Id.ToString());
            }



            // إذا لم يكن MFA مفعلًا، توليد JWT مباشرة
            var token = _jwtTokenService.GenerateJwtToken(user, out var expiry);
            var refreshToken = await _refreshTokenService.GenerateRefreshTokenAsync(user);

            return (true, new LoginResponseDto
            {
                Token = token,
                RefreshToken = refreshToken,
                TokenExpiry = expiry
            }, null, null); // user.Id.ToString()  لارجاع UserId
        }

        public async Task<(bool Success, LoginResponseDto? Response, string? Error)> RefreshTokenAsync(string token, string refreshToken)
        {
            var storedToken = await _dbContext.RefreshTokens
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.Token == refreshToken && !r.Revoked);

            if (storedToken == null || storedToken.ExpiryDate < DateTime.UtcNow)
                return (false, null, "Invalid or expired refresh token");

            var user = storedToken.User;
            var newToken = _jwtTokenService.GenerateJwtToken(user, out var expiry);
            var newRefreshToken = await _refreshTokenService.GenerateRefreshTokenAsync(user);

            // Revoke old refresh token
            storedToken.Revoked = true;
            await _dbContext.SaveChangesAsync();

            return (true, new LoginResponseDto
            {
                Token = newToken,
                RefreshToken = newRefreshToken,
                TokenExpiry = expiry
            }, null);
        }


        // Change User Pasword
        public async Task<(bool Success, string? Error)> ChangePasswordAsync(string userId, string currentPassword, string newPassword)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null || user.AuthType != ApplicationUser.AuthenticationType.Database)
                return (false, "User not found or invalid authentication type");

            //  التحقق من كلمة المرور الحالية
            var isCurrentValid = await _userManager.CheckPasswordAsync(user, currentPassword);
            if (!isCurrentValid)
                return (false, "Current password is incorrect");

            //  التحقق من أن الجديدة ليست نفس الحالية
            if (await _userManager.CheckPasswordAsync(user, newPassword))
                return (false, "New password cannot be the same as the current password");

            //  التحقق من أن كلمة المرور الجديدة لم تُستخدم مسبقاً
            var passwordHasher = new PasswordHasher<ApplicationUser>();
            var previousPasswords = await _dbContext.UserPasswordHistories
                .Where(h => h.UserId == user.Id)
                .OrderByDescending(h => h.CreatedAt)
                .Take(5) // 👈 التحقق من آخر 5 كلمات مرور
                .ToListAsync();

            foreach (var oldPassword in previousPasswords)
            {
                var verification = passwordHasher.VerifyHashedPassword(user, oldPassword.PasswordHash, newPassword);
                if (verification == PasswordVerificationResult.Success)
                    return (false, "You cannot reuse a previously used password");
            }

            //  إذا اجتاز التحقق، غيّر كلمة المرور
            var result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
            if (!result.Succeeded)
                return (false, string.Join(", ", result.Errors.Select(e => e.Description)));

            //  حفظ كلمة المرور الجديدة في سجل التاريخ
            var newHistory = new UserPasswordHistory
            {
                UserId = user.Id,
                PasswordHash = user.PasswordHash!,
                CreatedAt = DateTime.UtcNow
            };
            _dbContext.UserPasswordHistories.Add(newHistory);

            //  الإبقاء على آخر 5 فقط
            if (previousPasswords.Count >= 5)
            {
                var toRemove = previousPasswords.Skip(4); // احتفظ بـ 5 فقط
                _dbContext.UserPasswordHistories.RemoveRange(toRemove);
            }

            await _dbContext.SaveChangesAsync();

            return (true, null);
        }

        public async Task<(bool Success, string? Error)> DeactivateUserAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return (false, "User not found");

            if (!user.IsActive)
                return (false, "User is already deactivated");

            // تعطيل المستخدم
            user.IsActive = false;
            await _userManager.UpdateAsync(user);

            // تسجيل العملية في الـ Audit
            //await _auditService.LogAsync(new AuditLog
            //{
            //    UserId = User.Identity.Name,
            //    Action = "DeactivateUser",
            //    TargetUserId = user.Id,
            //    Timestamp = DateTime.UtcNow,
            //    Description = $"User {user.UserName} has been deactivated"
            //});

            return (true, null);

        }

        private string HashCode(string code, string salt)
        {
            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(salt));
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(code));
            return Convert.ToBase64String(hash);
        }

        private string GenerateNumericCode(int digits = 6)
        {
            var bytes = new byte[4];
            RandomNumberGenerator.Fill(bytes);
            uint random = BitConverter.ToUInt32(bytes, 0) % (uint)Math.Pow(10, digits);
            return random.ToString($"D{digits}");
        }

    }

}
