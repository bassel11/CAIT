using CommitteeApplication.Interfaces.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace CommitteeApplication.Authorization
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
            var userId = context.User.FindFirst("uid")?.Value;
            if (userId == null)
            {
                context.Fail();
                return;
            }

            var http = _contextAccessor.HttpContext;
            Guid? resourceId = null;
            if (http?.Items.TryGetValue("ResourceId", out var r) == true && r is Guid g)
                resourceId = g;

            var allowed = await _permissionService.HasPermissionAsync(Guid.Parse(userId), requirement.PermissionName, resourceId);
            if (allowed)
                context.Succeed(requirement);
            else
                context.Fail();
        }
    }
}
