using Identity.Core.Entities;

namespace Identity.Application.Interfaces
{
    public interface IRefreshTokenService
    {
        // توليد توكن جديد كلياً (عند تسجيل الدخول)
        Task<string> GenerateRefreshTokenAsync(ApplicationUser user);

        // تدوير التوكن (عند التجديد): يبطل القديم ويصدر جديداً
        Task<(bool Success, string? NewToken, string? Error)> RotateRefreshTokenAsync(string oldToken, ApplicationUser user);

        // إبطال التوكن (عند تسجيل الخروج)
        Task<bool> RevokeTokenAsync(string token);
    }
}
