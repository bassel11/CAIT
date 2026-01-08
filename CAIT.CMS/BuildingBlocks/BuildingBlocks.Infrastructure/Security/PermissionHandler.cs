using BuildingBlocks.Shared.Authorization;
using BuildingBlocks.Shared.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace BuildingBlocks.Infrastructure.Security
{
    public class PermissionHandler : AuthorizationHandler<PermissionRequirement>
    {
        private readonly IPermissionService _permissionService;
        private readonly ICurrentUserService _currentUser;
        private readonly IHttpContextAccessor _contextAccessor;

        public PermissionHandler(
            IPermissionService permissionService,
            ICurrentUserService currentUser,
            IHttpContextAccessor contextAccessor)
        {
            _permissionService = permissionService;
            _currentUser = currentUser;
            _contextAccessor = contextAccessor;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
        {
            if (!_currentUser.IsAuthenticated)
            {
                context.Fail();
                return;
            }

            // السوبر أدمن يتجاوز كل الفحوصات
            if (_currentUser.IsSuperAdmin)
            {
                context.Succeed(requirement);
                return;
            }

            var http = _contextAccessor.HttpContext;

            // 1. استخراج ResourceId من الـ Items (التي عبأها Middleware)
            Guid? resourceId = null;
            if (http?.Items.TryGetValue("ResourceId", out var r) == true && r is Guid g)
            {
                resourceId = g;
            }

            // 2. استخراج ParentResourceId (الجديد) ✅
            Guid? parentResourceId = null;
            if (http?.Items.TryGetValue("ParentResourceId", out var pr) == true && pr is Guid pg)
            {
                parentResourceId = pg;
            }

            try
            {
                // تمرير القيمتين للخدمة
                bool hasPermission = await _permissionService.HasPermissionAsync(
                    _currentUser.UserId,
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
            catch
            {
                context.Fail();
            }
        }
    }
}