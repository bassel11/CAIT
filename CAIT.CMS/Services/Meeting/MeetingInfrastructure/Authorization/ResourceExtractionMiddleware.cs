using Microsoft.AspNetCore.Http;

namespace MeetingInfrastructure.Authorization
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

            // من المسار (Route)
            //if (context.Request.RouteValues.TryGetValue("resourceId", out var rv) &&
            //    Guid.TryParse(rv?.ToString(), out var routeId))
            //    resourceId = routeId;

            // من الاستعلام (Query string)
            if (resourceId == null &&
                context.Request.Query.TryGetValue("resourceId", out var qv) &&
                Guid.TryParse(qv.FirstOrDefault(), out var queryId))
                resourceId = queryId;

            // من الهيدر (Header)
            if (resourceId == null &&
                context.Request.Headers.TryGetValue("X-ResourceId", out var hv) &&
                Guid.TryParse(hv.FirstOrDefault(), out var headerId))
                resourceId = headerId;

            if (resourceId != null)
                context.Items["ResourceId"] = resourceId;

            await _next(context);
        }
    }

}
