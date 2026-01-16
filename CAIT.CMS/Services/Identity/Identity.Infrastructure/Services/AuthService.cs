using BuildingBlocks.Contracts.SecurityEvents;
using Identity.Application.DTOs;
using Identity.Application.Interfaces;
using Identity.Application.Security;
using Identity.Core.Entities;
using Identity.Core.Enums;
using Identity.Infrastructure.Data;
using MassTransit;
using Microsoft.AspNetCore.Http;
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
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly ILoginSecurityService _loginSecurityService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IPublishEndpoint _publishEndpoint;

        public AuthService(UserManager<ApplicationUser> userManager,
                           ApplicationDbContext dbContext,
                           IJwtTokenService jwtTokenService,
                           IRefreshTokenService refreshTokenService,
                           IEmailService emailService,
                           RoleManager<ApplicationRole> roleManager,
                           ILoginSecurityService loginSecurityService,
                           IHttpContextAccessor httpContextAccessor,
                           IPublishEndpoint publishEndpoint)
        {
            _userManager = userManager;
            _dbContext = dbContext;
            _jwtTokenService = jwtTokenService;
            _refreshTokenService = refreshTokenService;
            _emailService = emailService;
            _roleManager = roleManager;
            _loginSecurityService = loginSecurityService;
            _httpContextAccessor = httpContextAccessor;
            _publishEndpoint = publishEndpoint;
        }

        public async Task<(bool Success, LoginResponseDto? Response, IEnumerable<string>? Errors)> RegisterAsync(RegisterDto dto)
        {

            if (dto.UserType != UserType.InternalEmployee && dto.UserType != UserType.SystemAccount)
            {
                return (false, null, new[] {
            "Invalid User Type. Registration via this endpoint is restricted to 'InternalEmployee' or 'SystemAccount' only."
             });
            }

            var user = new ApplicationUser
            {
                UserName = dto.Email,
                Email = dto.Email,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                UserType = dto.UserType,
                AuthType = AuthenticationType.Database,
                MFAEnabled = false,
                EmailConfirmed = true,
                IsActive = true,
                PrivilageType = dto.privilageType
            };

            //  تحقق مبدئي: إذا كان PrivilageType = PredifinedRoles، تأكد أن الدور صالح
            if (dto.privilageType == PrivilageType.PredifinedRoles)
            {
                if (string.IsNullOrWhiteSpace(dto.Role))
                {
                    return (false, null, new[] { "Role must be provided when PrivilageType is PredifinedRoles" });
                }

                if (!await _roleManager.RoleExistsAsync(dto.Role))
                {
                    return (false, null, new[] { $"Role '{dto.Role}' does not exist" });
                }
            }

            //  إنشاء المستخدم
            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
                return (false, null, result.Errors.Select(e => e.Description));

            //  فقط إذا كان PrivilageType = PredifinedRoles أضف الدور
            if (dto.privilageType == PrivilageType.PredifinedRoles)
            {
                await _userManager.AddToRoleAsync(user, dto.Role);
            }

            //  سجل كلمة المرور في التاريخ
            var passwordHistory = new UserPasswordHistory
            {
                UserId = user.Id,
                PasswordHash = user.PasswordHash!,
                CreatedAt = DateTime.UtcNow
            };

            _dbContext.UserPasswordHistories.Add(passwordHistory);
            await _dbContext.SaveChangesAsync();

            //  توليد التوكنات
            var jwtResult = await _jwtTokenService.GenerateJwtTokenAsync(user);
            var refreshToken = await _refreshTokenService.GenerateRefreshTokenAsync(user);

            var response = new LoginResponseDto
            {
                Token = jwtResult.Token,
                RefreshToken = refreshToken,
                TokenExpiry = jwtResult.Expiry
            };

            return (true, response, null);
        }

        public async Task<(bool Success, LoginResponseDto? Response, string? Error, string? UserId)> LoginAsync(LoginDto dto)
        {
            // البحث عن المستخدم مع مراعاة نوع المصادقة Database فقط
            var user = await _userManager.Users
                .FirstOrDefaultAsync(u => u.Email == dto.Email && u.AuthType == ApplicationUser.AuthenticationType.Database);

            if (user == null || !await _userManager.CheckPasswordAsync(user, dto.Password))
            {
                if (user != null)
                    await _loginSecurityService.HandleFailedLoginAsync(user, GetIp());

                return (false, null, "Invalid credentials", null);
            }

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

            //  Login ناجح بالكامل → تصفير AccessFailedCount
            if (user.AccessFailedCount > 0)
            {
                user.AccessFailedCount = 0;
                await _userManager.UpdateAsync(user);
            }

            // إذا لم يكن MFA مفعلًا، توليد JWT مباشرة
            var jwtResult = await _jwtTokenService.GenerateJwtTokenAsync(user);
            var refreshToken = await _refreshTokenService.GenerateRefreshTokenAsync(user);

            return (true, new LoginResponseDto
            {
                Token = jwtResult.Token,
                RefreshToken = refreshToken,
                TokenExpiry = jwtResult.Expiry
            }, null, null); // user.Id.ToString()  لارجاع UserId
        }

        // ... (بنفس الـ Constructor والحقول السابقة)

        public async Task<(bool Success, LoginResponseDto? Response, string? Error)> RefreshTokenAsync(string expiredToken, string refreshToken)
        {
            // 1. استخراج الـ UserId من التوكن المنتهي (دون التحقق من الوقت)
            // ملاحظة: تأكد أن JwtTokenService لديك يحتوي على دالة GetPrincipalFromExpiredToken
            // أو يمكنك الاعتماد على الـ UserId المرتبط بالـ RefreshToken في الداتابيس (أكثر أماناً)

            // هنا سنبحث عن الـ Refresh Token أولاً لنعرف المستخدم
            var storedTokenEntity = await _dbContext.RefreshTokens
                 .Include(r => r.User)
                 .FirstOrDefaultAsync(r => r.Token == refreshToken);

            if (storedTokenEntity == null) return (false, null, "Invalid Token");

            var user = storedTokenEntity.User;

            // 2. استدعاء خدمة التدوير الآمنة
            var rotationResult = await _refreshTokenService.RotateRefreshTokenAsync(refreshToken, user);

            if (!rotationResult.Success)
                return (false, null, rotationResult.Error);

            // 3. توليد Access Token جديد
            var newJwt = await _jwtTokenService.GenerateJwtTokenAsync(user);

            return (true, new LoginResponseDto
            {
                Token = newJwt.Token,
                RefreshToken = rotationResult.NewToken!, // التوكن الجديد
                TokenExpiry = newJwt.Expiry
            }, null);
        }

        public async Task<(bool Success, string? Error)> LogoutAsync(string refreshToken)
        {
            // 1. العثور على التوكن
            var storedToken = await _dbContext.RefreshTokens
                .FirstOrDefaultAsync(r => r.Token == refreshToken);

            if (storedToken == null) return (true, null);

            var userId = storedToken.UserId;

            // 2. إبطال كافة التوكنات (تنظيف الجلسات)
            var userTokens = await _dbContext.RefreshTokens
                .Where(t => t.UserId == userId && !t.Revoked)
                .ToListAsync();

            foreach (var token in userTokens)
            {
                token.Revoked = true;
                token.RevokedAt = DateTime.UtcNow;
                token.ReasonRevoked = "User Logout (All Sessions)";
                token.RevokedByIp = GetIp();
            }
            _dbContext.RefreshTokens.UpdateRange(userTokens);

            // 🔥 3. تدوير بصمة الأمان (Security Stamp)
            // هذا يجعل التوكنات القديمة (Access Tokens) غير صالحة فوراً حتى لو لم تنتهِ مدتها
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user != null)
            {
                await _userManager.UpdateSecurityStampAsync(user);
            }

            // 4. نشر الحدث لحذف الكاش في الخدمات الأخرى
            await _publishEndpoint.Publish(new UserLoggedOutEvent
            {
                UserId = userId,
                Timestamp = DateTime.UtcNow
            });

            await _dbContext.SaveChangesAsync();

            return (true, null);
        }

        // ... (باقي الدوال Register, Login, ChangePassword كما هي)

        // Change User Pasword
        public async Task<(bool Success, string? Error)> ChangePasswordAsync(string userId, string currentPassword, string newPassword)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null || user.AuthType != ApplicationUser.AuthenticationType.Database)
                return (false, "User not found or invalid authentication type");

            // 1. التحقق من كلمة المرور الحالية
            var isCurrentValid = await _userManager.CheckPasswordAsync(user, currentPassword);
            if (!isCurrentValid)
                return (false, "Current password is incorrect");

            // 2. التحقق من أن الجديدة ليست نفس الحالية
            if (await _userManager.CheckPasswordAsync(user, newPassword))
                return (false, "New password cannot be the same as the current password");

            // 3. التحقق من سجل كلمات المرور (Prevent Reuse)
            var passwordHasher = new PasswordHasher<ApplicationUser>();
            var previousPasswords = await _dbContext.UserPasswordHistories
                .Where(h => h.UserId == user.Id)
                .OrderByDescending(h => h.CreatedAt)
                .Take(5)
                .ToListAsync();

            foreach (var oldPassword in previousPasswords)
            {
                var verification = passwordHasher.VerifyHashedPassword(user, oldPassword.PasswordHash, newPassword);
                if (verification == PasswordVerificationResult.Success)
                    return (false, "You cannot reuse a previously used password");
            }

            // 4. تغيير كلمة المرور فعلياً
            // (تقوم تلقائياً بتحديث SecurityStamp مما يقتل الـ Access Tokens)
            var result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);

            if (!result.Succeeded)
                return (false, string.Join(", ", result.Errors.Select(e => e.Description)));

            // 5. حفظ في السجل (Password History)
            var newHistory = new UserPasswordHistory
            {
                UserId = user.Id,
                PasswordHash = user.PasswordHash!,
                CreatedAt = DateTime.UtcNow
            };
            _dbContext.UserPasswordHistories.Add(newHistory);

            // تنظيف السجل القديم (Retention Policy)
            // نستخدم RemoveRange لضمان حذف أي عدد زائد (أفضل من حذف واحد فقط)
            if (previousPasswords.Count >= 5)
            {
                var toRemove = previousPasswords.Skip(4).ToList(); // نحتفظ بأحدث 4 + الجديد الذي أضفناه = 5
                if (toRemove.Any())
                {
                    _dbContext.UserPasswordHistories.RemoveRange(toRemove);
                }
            }

            //  حرق كافة الجلسات (Refresh Tokens) 

            var activeRefreshTokens = await _dbContext.RefreshTokens
                .Where(t => t.UserId == user.Id && !t.Revoked)
                .ToListAsync();

            foreach (var token in activeRefreshTokens)
            {
                token.Revoked = true;
                token.RevokedAt = DateTime.UtcNow;
                token.ReasonRevoked = "Password Changed";
            }

            // 7. حفظ التغييرات دفعة واحدة (History + Tokens)
            await _dbContext.SaveChangesAsync();

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

        private string GetIp()
        {
            return _httpContextAccessor.HttpContext?
                .Connection.RemoteIpAddress?.ToString() ?? "Unknown";
        }
    }

}
