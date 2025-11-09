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

            // محاولة الحصول على ResourceId من HttpContext
            var httpContext = _httpContextAccessor.HttpContext;
            Guid? resourceId = null;

            if (httpContext != null)
            {
                // من query string
                if (httpContext.Request.Query.TryGetValue("resourceId", out var qValues) && Guid.TryParse(qValues.FirstOrDefault(), out var parsedQueryId))
                    resourceId = parsedQueryId;

                // من header (أولوية أعلى)
                if (httpContext.Request.Headers.TryGetValue("X-ResourceId", out var hValues) && Guid.TryParse(hValues.FirstOrDefault(), out var parsedHeaderId))
                    resourceId = parsedHeaderId;
            }

            bool hasPermission = await _permissionChecker.HasPermissionAsync(userId, requirement.PermissionName, resourceId);

            if (hasPermission)
                context.Succeed(requirement);
            else
                context.Fail();
        }
    }
}