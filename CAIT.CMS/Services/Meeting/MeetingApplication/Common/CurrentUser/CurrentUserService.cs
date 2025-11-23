using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace MeetingApplication.Common.CurrentUser
{
    public class CurrentUserService
    {
        #region Fields

        private readonly IHttpContextAccessor _httpContextAccessor;
        private ClaimsPrincipal? User => _httpContextAccessor.HttpContext?.User;

        #endregion


        #region Constructor
        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        #endregion

        public Guid UserId
        {
            get
            {
                var uid = User?.FindFirst("uid")?.Value;
                if (Guid.TryParse(uid, out var id)) return id;
                return Guid.Empty;
            }
        }

        public string? UserName => User?.Identity?.Name;

        public string? Email => User?.FindFirst(ClaimTypes.Email)?.Value;

        public IEnumerable<string> Roles =>
            User?.FindAll(ClaimTypes.Role).Select(c => c.Value) ?? Enumerable.Empty<string>();

        //public IEnumerable<string> Permissions
        //{
        //    get
        //    {
        //        var raw = User?.FindFirst("permissions")?.Value;
        //        if (string.IsNullOrWhiteSpace(raw)) return Enumerable.Empty<string>();
        //        return raw.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim());
        //    }
        //}

        public bool IsAuthenticated => User?.Identity?.IsAuthenticated ?? false;

        public bool IsSuperAdmin =>
            string.Equals(User?.FindFirst("is_superadmin")?.Value, "true", StringComparison.OrdinalIgnoreCase) ||
            Roles.Any(r => string.Equals(r, "SuperAdmin", StringComparison.OrdinalIgnoreCase));

        public bool IsInRole(string role)
        {
            if (string.IsNullOrWhiteSpace(role)) return false;
            return Roles.Any(r => string.Equals(r, role, StringComparison.OrdinalIgnoreCase));
        }

        //public bool HasPermission(string permission)
        //{
        //    if (string.IsNullOrWhiteSpace(permission)) return false;
        //    return Permissions.Any(p => string.Equals(p, permission, StringComparison.OrdinalIgnoreCase));
        //}
    }
}
