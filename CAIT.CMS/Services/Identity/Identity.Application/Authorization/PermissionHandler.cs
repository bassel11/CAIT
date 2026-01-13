using BuildingBlocks.Shared.Services; // أو المكان الذي يوجد فيه ICurrentUserService
using Identity.Application.Interfaces.Authorization; // حيث يوجد IPermissionChecker
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace Identity.Application.Authorization
{
    public class PermissionHandler : AuthorizationHandler<PermissionRequirement>
    {
        // نستخدم Checker لأنه الأدق داخل خدمة Identity
        private readonly IPermissionChecker _permissionChecker;
        // نستخدم CurrentUser للنظافة وتوحيد جلب الـ ID
        private readonly ICurrentUserService _currentUser;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PermissionHandler(
            IPermissionChecker permissionChecker,
            ICurrentUserService currentUser,
            IHttpContextAccessor httpContextAccessor)
        {
            _permissionChecker = permissionChecker;
            _currentUser = currentUser;
            _httpContextAccessor = httpContextAccessor;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
        {
            // 1. التحقق من المصادقة باستخدام التجريد
            if (!_currentUser.IsAuthenticated)
            {
                // لا نفعل شيئاً (يترك الأمر للـ default behavior) أو Fail
                return;
            }

            // 2. تحسين الأداء: السوبر أدمن يدخل فوراً (Best Practice)
            // ملاحظة: تأكد أن ICurrentUserService يقرأ الـ Role بشكل صحيح
            if (_currentUser.Roles.Contains("SuperAdmin") && _currentUser.IsSuperAdmin)
            {
                context.Succeed(requirement);
                return;
            }

            // 3. استخراج الموارد
            var httpContext = _httpContextAccessor.HttpContext;
            Guid? resourceId = null;
            Guid? parentResourceId = null;

            if (httpContext != null)
            {
                if (httpContext.Items.TryGetValue("ResourceId", out var rVal) && rVal is Guid rGuid)
                    resourceId = rGuid;

                if (httpContext.Items.TryGetValue("ParentResourceId", out var pVal) && pVal is Guid pGuid)
                    parentResourceId = pGuid;
            }

            try
            {
                // 4. الفحص الفعلي
                bool hasPermission = await _permissionChecker.HasPermissionAsync(
                    _currentUser.UserId, // استخدام الـ ID الموحد
                    requirement.PermissionName,
                    resourceId,
                    parentResourceId
                );

                if (hasPermission)
                {
                    context.Succeed(requirement);
                }
                else
                {
                    context.Fail();
                }
            }
            catch (Exception)
            {
                // في حالة خطأ في الاتصال بالقاعدة، نرفض الطلب بأمان
                context.Fail();
            }
        }
    }
}