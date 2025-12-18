using Microsoft.AspNetCore.Http;

namespace BuildingBlocks.Infrastructure.Security
{
    // 1. يجب أن يرث من IMiddleware
    public class ResourceExtractionMiddleware : IMiddleware
    {
        // 2. نحذف RequestDelegate من الكونستركتور
        // الكونستركتور يبقى نظيفاً (أو تحقن فيه خدمات أخرى مثل ILogger)
        public ResourceExtractionMiddleware()
        {
        }

        // 3. نعدل دالة InvokeAsync لتستقبل (context, next)
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            Guid? resourceId = null;

            // 1. Query String
            if (context.Request.Query.TryGetValue("resourceId", out var qv) &&
                Guid.TryParse(qv.FirstOrDefault(), out var queryId))
            {
                resourceId = queryId;
            }

            // 2. Headers
            if (resourceId == null &&
                context.Request.Headers.TryGetValue("X-ResourceId", out var hv) &&
                Guid.TryParse(hv.FirstOrDefault(), out var headerId))
            {
                resourceId = headerId;
            }

            // 3. Route Values
            if (resourceId == null &&
                context.Request.RouteValues.TryGetValue("resourceId", out var rv) &&
                Guid.TryParse(rv?.ToString(), out var routeId))
            {
                resourceId = routeId;
            }

            if (resourceId != null)
            {
                context.Items["ResourceId"] = resourceId;
            }

            // استدعاء الميدل وير التالي
            await next(context);
        }
    }
}