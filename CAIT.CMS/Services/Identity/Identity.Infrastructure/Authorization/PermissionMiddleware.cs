using Identity.Application.Authorization;
using Identity.Application.Interfaces.Authorization;
using Microsoft.AspNetCore.Http;

namespace Identity.Infrastructure.Authorization
{
    public class PermissionMiddleware
    {
        private readonly RequestDelegate _next;

        public PermissionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, IPermissionChecker permissionChecker)
        {
            var endpoint = context.GetEndpoint();
            var requiredPermissions = endpoint?.Metadata.GetOrderedMetadata<PermissionRequirement>();

            if (requiredPermissions != null && requiredPermissions.Any())
            {
                var userIdClaim = context.User.FindFirst("uid")?.Value;
                if (string.IsNullOrEmpty(userIdClaim))
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await context.Response.WriteAsync("User not authenticated");
                    return;
                }

                var userId = Guid.Parse(userIdClaim);

                // ==============================
                //  استخراج ResourceId من 3 أماكن
                // ==============================
                Guid? resourceId = null;

                //  من Route (مثل /meeting/{resourceId}/create)
                if (context.Request.RouteValues.TryGetValue("resourceId", out var routeValue))
                {
                    if (Guid.TryParse(routeValue?.ToString(), out var parsedRouteId))
                        resourceId = parsedRouteId;
                }

                //  من Query string (?resourceId=...)
                if (resourceId == null &&
                    context.Request.Query.TryGetValue("resourceId", out var qValues) &&
                    Guid.TryParse(qValues.FirstOrDefault(), out var parsedQueryId))
                {
                    resourceId = parsedQueryId;
                }

                //  من Header (X-ResourceId)
                if (resourceId == null &&
                    context.Request.Headers.TryGetValue("X-resourceId", out var hValues) &&
                    Guid.TryParse(hValues.FirstOrDefault(), out var parsedHeaderId))
                {
                    resourceId = parsedHeaderId;
                }

                // ==============================
                //  تحقق من الصلاحية المطلوبة
                // ==============================
                foreach (var permission in requiredPermissions)
                {
                    bool hasPermission = await permissionChecker.HasPermissionAsync(userId, permission.PermissionName, resourceId);
                    if (!hasPermission)
                    {
                        context.Response.StatusCode = StatusCodes.Status403Forbidden;
                        await context.Response.WriteAsync($"Access denied: missing permission {permission.PermissionName}");
                        return;
                    }
                }
            }

            await _next(context);
        }
    }
}
