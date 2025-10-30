using Identity.Application.Interfaces.Authorization;
using Microsoft.AspNetCore.Authorization;

namespace Identity.Application.Authorization
{
    public class PermissionHandler : AuthorizationHandler<PermissionRequirement>
    {
        private readonly IPermissionChecker _checker;

        public PermissionHandler(IPermissionChecker checker)
        {
            _checker = checker;
        }

        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            PermissionRequirement requirement)
        {
            var userIdClaim = context.User.FindFirst("uid");
            if (userIdClaim == null)
            {
                context.Fail();
                return;
            }

            if (context.User.HasClaim("is_superadmin", "true"))
            {
                context.Succeed(requirement);
                return;
            }

            if (!Guid.TryParse(userIdClaim.Value, out var userId))
            {
                context.Fail();
                return;
            }

            var allowed = await _checker.HasPermissionAsync(userId, requirement.PermissionName, requirement.CommitteeId);
            if (allowed)
                context.Succeed(requirement);
            else
                context.Fail();
        }
    }
}
