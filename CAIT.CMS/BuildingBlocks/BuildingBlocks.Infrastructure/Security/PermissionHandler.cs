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

            // 1. سوبر أدمن (مركزي)
            if (_currentUser.IsSuperAdmin)
            {
                context.Succeed(requirement);
                return;
            }

            // 2. استخراج ResourceId
            Guid? resourceId = null;
            var http = _contextAccessor.HttpContext;
            // البحث في Items (من Middleware)
            if (http?.Items.TryGetValue("ResourceId", out var r) == true && r is Guid g)
            {
                resourceId = g;
            }

            // 3. الفحص
            try
            {
                if (await _permissionService.HasPermissionAsync(_currentUser.UserId, requirement.PermissionName, resourceId))
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
