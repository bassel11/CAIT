using Identity.Application.Interfaces.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace Identity.Application.Authorization
{
    public class PermissionHandler : AuthorizationHandler<PermissionRequirement>
    {
        private readonly IPermissionChecker _permissionChecker;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PermissionHandler(IPermissionChecker permissionChecker, IHttpContextAccessor httpContextAccessor)
        {
            _permissionChecker = permissionChecker;
            _httpContextAccessor = httpContextAccessor;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
        {
            var userIdClaim = context.User.FindFirst("uid")?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
            {
                context.Fail();
                return;
            }

            var userId = Guid.Parse(userIdClaim);
            bool hasPermission = await _permissionChecker.HasPermissionAsync(userId, requirement.PermissionName);

            if (hasPermission)
                context.Succeed(requirement);
            else
                context.Fail();
        }
    }
}