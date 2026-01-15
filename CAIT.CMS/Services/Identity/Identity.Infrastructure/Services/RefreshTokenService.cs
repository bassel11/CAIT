using Identity.Application.Interfaces;
using Identity.Core.Entities;
using Identity.Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;

namespace Identity.Infrastructure.Services
{
    public class RefreshTokenService : IRefreshTokenService
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public RefreshTokenService(
            ApplicationDbContext context,
            IConfiguration configuration,
            IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<string> GenerateRefreshTokenAsync(ApplicationUser user)
        {
            var token = GenerateRandomTokenString();
            var days = _configuration.GetValue<int>("Jwt:RefreshTokenValidityInDays", 7);

            var refreshToken = new RefreshToken
            {
                UserId = user.Id,
                Token = token,
                ExpiryDate = DateTime.UtcNow.AddDays(days),
                CreatedByIp = GetIpAddress(),
                CreatedAt = DateTime.UtcNow,
                IsUsed = false,
                Revoked = false
            };

            _context.RefreshTokens.Add(refreshToken);
            await _context.SaveChangesAsync();

            return token;
        }

        public async Task<(bool Success, string? NewToken, string? Error)> RotateRefreshTokenAsync(string oldToken, ApplicationUser user)
        {
            var storedToken = await _context.RefreshTokens
                .FirstOrDefaultAsync(x => x.Token == oldToken);

            // 1. التحقق من الوجود
            if (storedToken == null)
                return (false, null, "Invalid Token");

            // 2. كشف السرقة (Security: Reuse Detection) 🚨
            if (storedToken.IsUsed)
            {
                // إذا تم استخدام التوكن سابقاً، فهذا يعني محاولة اختراق
                // الإجراء: إبطال كافة توكنات هذا المستخدم فوراً!
                await RevokeAllTokensForUserAsync(storedToken.UserId, "Replay Attack Detected");
                return (false, null, "Security Alert: Token reuse detected. All sessions revoked.");
            }

            // 3. التحقق من الصلاحية
            if (storedToken.Revoked)
                return (false, null, "Token is revoked");

            if (storedToken.IsExpired)
                return (false, null, "Token is expired");

            // 4. تدوير التوكن (Rotation)
            storedToken.IsUsed = true;
            storedToken.RevokedAt = DateTime.UtcNow;
            storedToken.RevokedByIp = GetIpAddress();

            var newTokenString = GenerateRandomTokenString();
            var days = _configuration.GetValue<int>("Jwt:RefreshTokenValidityInDays", 7);

            var newRefreshToken = new RefreshToken
            {
                UserId = user.Id,
                Token = newTokenString,
                ExpiryDate = DateTime.UtcNow.AddDays(days),
                CreatedByIp = GetIpAddress(),
                CreatedAt = DateTime.UtcNow,
                IsUsed = false,
                Revoked = false
            };

            // ربط السلسلة (Audit Trail)
            storedToken.ReplacedByToken = newTokenString;

            _context.RefreshTokens.Add(newRefreshToken);
            _context.RefreshTokens.Update(storedToken);
            await _context.SaveChangesAsync();

            return (true, newTokenString, null);
        }

        public async Task<bool> RevokeTokenAsync(string token)
        {
            var storedToken = await _context.RefreshTokens.FirstOrDefaultAsync(x => x.Token == token);

            // إذا لم يوجد أو كان غير فعال، نعتبر العملية ناجحة (Idempotent)
            //if (storedToken == null || !storedToken.IsActive)
            //    return true;

            // إذا لم يوجد أو انتهت صلاحيته، العملية تعتبر ناجحة
            if (storedToken == null || storedToken.Revoked || storedToken.ExpiryDate <= DateTime.UtcNow)
                return true;

            storedToken.Revoked = true;
            storedToken.RevokedAt = DateTime.UtcNow;
            storedToken.RevokedByIp = GetIpAddress();
            storedToken.ReasonRevoked = "Logout";

            _context.RefreshTokens.Update(storedToken);
            await _context.SaveChangesAsync();

            return true;
        }

        // --- Helpers ---
        private async Task RevokeAllTokensForUserAsync(Guid userId, string reason)
        {
            //var tokens = await _context.RefreshTokens
            //    .Where(t => t.UserId == userId && t.IsActive) // فقط الفعالة
            //    .ToListAsync();

            var tokens = await _context.RefreshTokens
                .Where(t => t.UserId == userId && !t.Revoked && t.ExpiryDate > DateTime.UtcNow)
                .ToListAsync();

            foreach (var t in tokens)
            {
                t.Revoked = true;
                t.RevokedAt = DateTime.UtcNow;
                t.ReasonRevoked = reason;
                t.RevokedByIp = GetIpAddress();
            }
            if (tokens.Any()) await _context.SaveChangesAsync();
        }

        private string GenerateRandomTokenString()
        {
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        private string GetIpAddress()
        {
            if (_httpContextAccessor.HttpContext?.Request.Headers.ContainsKey("X-Forwarded-For") == true)
                return _httpContextAccessor.HttpContext.Request.Headers["X-Forwarded-For"].ToString();

            return _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
        }
    }
}