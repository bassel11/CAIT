using Microsoft.AspNetCore.Http;

namespace Identity.Infrastructure.Authorization
{
    public class ResourceExtractionMiddleware
    {
        private readonly RequestDelegate _next;

        public ResourceExtractionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            Guid? resourceId = null;
            Guid? parentResourceId = null;

            if (context.Request.RouteValues.TryGetValue("resourceId", out var rv) &&
                Guid.TryParse(rv?.ToString(), out var routeId))
                resourceId = routeId;

            if (context.Request.Query.TryGetValue("resourceId", out var qv) &&
                Guid.TryParse(qv.FirstOrDefault(), out var queryId))
                resourceId ??= queryId;

            if (context.Request.Headers.TryGetValue("X-ResourceId", out var hv) &&
                Guid.TryParse(hv.FirstOrDefault(), out var headerId))
                resourceId ??= headerId;

            // parentResourceId من route أو query أو header

            if (context.Request.RouteValues.TryGetValue("parentResourceId", out var prv) &&
                Guid.TryParse(prv?.ToString(), out var pRouteId))
                parentResourceId = pRouteId;

            if (context.Request.Query.TryGetValue("parentResourceId", out var pqv) &&
                Guid.TryParse(pqv.FirstOrDefault(), out var pQueryId))
                parentResourceId = pQueryId;

            if (context.Request.Headers.TryGetValue("X-ParentResourceId", out var phv) &&
                Guid.TryParse(phv.FirstOrDefault(), out var pHeaderId))
                parentResourceId ??= pHeaderId;

            if (resourceId != null)
                context.Items["ResourceId"] = resourceId;

            if (parentResourceId != null)
                context.Items["ParentResourceId"] = parentResourceId;

            await _next(context);
        }
    }
}
