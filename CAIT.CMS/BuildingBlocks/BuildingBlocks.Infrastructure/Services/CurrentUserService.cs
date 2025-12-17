using BuildingBlocks.Shared.Services;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace BuildingBlocks.Infrastructure.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        private ClaimsPrincipal? User => _httpContextAccessor.HttpContext?.User;

        public Guid UserId
        {
            get
            {
                // 1. محاولة البحث عن claim باسم "uid" (المستخدم حالياً لديك)
                var idClaim = User?.FindFirst("uid")?.Value;

                // 2. إذا لم يوجد، نبحث عن Standard Subject (sub)
                if (string.IsNullOrEmpty(idClaim))
                {
                    idClaim = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                }

                // 3. إذا لم يوجد، نبحث عن "sub" الصريح
                if (string.IsNullOrEmpty(idClaim))
                {
                    idClaim = User?.FindFirst("sub")?.Value;
                }

                // محاولة التحويل لـ GUID
                return Guid.TryParse(idClaim, out var id) ? id : Guid.Empty;
            }
        }

        // الاسم: نبحث في Name ثم في الـ Claims الأخرى كاحتياط
        public string? UserName => User?.Identity?.Name
                                   ?? User?.FindFirst(ClaimTypes.Name)?.Value
                                   ?? User?.FindFirst("name")?.Value
                                   ?? "Anonymous";

        public string? Email => User?.FindFirst(ClaimTypes.Email)?.Value
                                ?? User?.FindFirst("email")?.Value;

        public bool IsAuthenticated => User?.Identity?.IsAuthenticated ?? false;

        public IEnumerable<string> Roles => User?.FindAll(ClaimTypes.Role)
                                                 .Select(c => c.Value)
                                                 ?? Enumerable.Empty<string>();

        // منطق SuperAdmin كما هو لديك، مع تحسين بسيط
        public bool IsSuperAdmin =>
            string.Equals(User?.FindFirst("is_superadmin")?.Value, "true", StringComparison.OrdinalIgnoreCase) ||
            IsInRole("SuperAdmin");

        public bool IsInRole(string role)
        {
            if (string.IsNullOrWhiteSpace(role)) return false;
            return User?.IsInRole(role) ?? false; // استخدام دالة ASP.NET الأصلية لأنها أسرع وأدق
        }
    }
}
