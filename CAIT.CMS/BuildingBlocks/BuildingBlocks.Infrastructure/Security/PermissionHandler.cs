using BuildingBlocks.Shared.Authorization;
using BuildingBlocks.Shared.Services; // تأكد من الـ Namespaces
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging; // ضروري للـ Logger

namespace BuildingBlocks.Infrastructure.Security
{
    public class PermissionHandler : AuthorizationHandler<PermissionRequirement>
    {
        private readonly IPermissionService _permissionService;
        private readonly ICurrentUserService _currentUser;
        private readonly IHttpContextAccessor _contextAccessor;
        // يفضل إضافة Logger لتتبع الأخطاء
        private readonly ILogger<PermissionHandler> _logger;

        public PermissionHandler(
            IPermissionService permissionService,
            ICurrentUserService currentUser,
            IHttpContextAccessor contextAccessor,
            ILogger<PermissionHandler> logger)
        {
            _permissionService = permissionService;
            _currentUser = currentUser;
            _contextAccessor = contextAccessor;
            _logger = logger;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
        {
            if (!_currentUser.IsAuthenticated)
            {
                context.Fail();
                return;
            }

            if (_currentUser.IsSuperAdmin)
            {
                context.Succeed(requirement);
                return;
            }

            var http = _contextAccessor.HttpContext;

            // استخراج ResourceId & ParentResourceId
            Guid? resourceId = null;
            if (http?.Items.TryGetValue("ResourceId", out var r) == true && r is Guid g) resourceId = g;

            Guid? parentResourceId = null;
            if (http?.Items.TryGetValue("ParentResourceId", out var pr) == true && pr is Guid pg) parentResourceId = pg;

            try
            {
                bool hasPermission = await _permissionService.HasPermissionAsync(
                    _currentUser.UserId,
                    requirement.PermissionName,
                    resourceId,
                    parentResourceId
                );

                if (hasPermission) context.Succeed(requirement);
                else context.Fail();
            }
            // 🔥 هذا هو التعديل المطلوب: التقاط خاص لـ UnauthorizedAccessException
            catch (UnauthorizedAccessException)
            {
                // نترك الاستثناء يصعد للأعلى لكي يمسكه الـ GlobalExceptionHandler
                // هذا سيوقف الـ Pipeline ويسمح بإرجاع 401
                throw;
            }
            catch (Exception ex)
            {
                // الأخطاء الأخرى نعتبرها فشل في الصلاحية (403) أو خطأ في السيرفر
                _logger.LogError(ex, "Error checking permission");
                context.Fail();
            }
        }
    }
}