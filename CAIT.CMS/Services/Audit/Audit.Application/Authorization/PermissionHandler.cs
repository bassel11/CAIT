using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace Audit.Application.Authorization
{
    public class PermissionHandler : AuthorizationHandler<PermissionRequirement>
    {
        private readonly IPermissionService _permissionService;
        private readonly IHttpContextAccessor _contextAccessor;

        public PermissionHandler(IPermissionService permissionService, IHttpContextAccessor contextAccessor)
        {
            _permissionService = permissionService;
            _contextAccessor = contextAccessor;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
        {
            var userIdClaim = context.User.FindFirst("uid")?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userGuid))
            {
                context.Fail();
                return;
            }

            // 2. تحقق من SuperAdmin — رول أو Claim
            // - IsInRole("SuperAdmin")
            // - أو Claim "Is_SuperAdmin" == "true" (أو "1")
            var isRoleSuperAdmin = context.User.IsInRole("SuperAdmin");
            var isClaimSuperAdmin = context.User.Claims.Any(c =>
                string.Equals(c.Type, "Is_SuperAdmin", StringComparison.OrdinalIgnoreCase) &&
                (string.Equals(c.Value, "true", StringComparison.OrdinalIgnoreCase) || c.Value == "1"));

            if (isRoleSuperAdmin || isClaimSuperAdmin)
            {
                context.Succeed(requirement);
                return; // لا تستدعي خدمة الصلاحيات
            }

            // 3. إذا لم يكن SuperAdmin استخرج resourceId من HttpContext (إن وجد)
            Guid? resourceId = null;
            var http = _contextAccessor.HttpContext;
            if (http?.Items.TryGetValue("ResourceId", out var r) == true && r is Guid g)
                resourceId = g;

            // 4. استدعاء خدمة الـ Permission الفعلية
            bool allowed;
            try
            {
                allowed = await _permissionService.HasPermissionAsync(userGuid, requirement.PermissionName, resourceId);
            }
            catch
            {
                // إذا حدث خطأ في الاتصال بخدمة الهوية: امنح الرفض (أو اختر السلوك الذي تريده)
                context.Fail();
                return;
            }

            if (allowed)
                context.Succeed(requirement);
            else
                context.Fail();
        }
    }
}
