using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace BuildingBlocks.Infrastructure.Security
{
    // 1. يرث من IMiddleware لدعم الحقن عبر DI Factory
    public class ResourceExtractionMiddleware : IMiddleware
    {
        private readonly ILogger<ResourceExtractionMiddleware> _logger;

        // 2. حقن Logger للمساعدة في التتبع (Best Practice)
        public ResourceExtractionMiddleware(ILogger<ResourceExtractionMiddleware> logger)
        {
            _logger = logger;
        }

        // 3. تنفيذ الدالة الأساسية
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            // =================================================================
            // أ) استخراج Resource Id
            // =================================================================
            // المفاتيح المتوقعة:
            // Query/Route: "resourceId"
            // Header: "X-ResourceId"
            var resourceId = ExtractGuid(context, keyName: "resourceId", headerName: "X-ResourceId");

            if (resourceId.HasValue)
            {
                context.Items["ResourceId"] = resourceId.Value;
                _logger.LogDebug("ResourceId extracted: {ResourceId}", resourceId);
            }

            // =================================================================
            // ب) استخراج Parent Resource Id (الجديد)
            // =================================================================
            // المفاتيح المتوقعة:
            // Query/Route: "parentResourceId"
            // Header: "X-ParentResourceId"
            var parentResourceId = ExtractGuid(context, keyName: "parentResourceId", headerName: "X-ParentResourceId");

            if (parentResourceId.HasValue)
            {
                context.Items["ParentResourceId"] = parentResourceId.Value;
                _logger.LogDebug("ParentResourceId extracted: {ParentResourceId}", parentResourceId);
            }

            // الانتقال للميدل وير التالي
            await next(context);
        }

        /// <summary>
        /// دالة مساعدة لاستخراج GUID من مصادر متعددة بترتيب أولوية محدد
        /// </summary>
        /// <param name="context">سياق الطلب</param>
        /// <param name="keyName">اسم المفتاح في الرابط أو الكويري</param>
        /// <param name="headerName">اسم المفتاح في الهيدر</param>
        /// <returns>Guid? or null</returns>
        private Guid? ExtractGuid(HttpContext context, string keyName, string headerName)
        {
            // 1. الأولوية الأولى: Query String
            // مثال: ?resourceId=...
            if (context.Request.Query.TryGetValue(keyName, out var queryVal) &&
                Guid.TryParse(queryVal.FirstOrDefault(), out var queryId))
            {
                return queryId;
            }

            // 2. الأولوية الثانية: Headers
            // مثال: X-ResourceId: ...
            if (context.Request.Headers.TryGetValue(headerName, out var headerVal) &&
                Guid.TryParse(headerVal.FirstOrDefault(), out var headerId))
            {
                return headerId;
            }

            // 3. الأولوية الثالثة: Route Values
            // مثال: api/tasks/{resourceId}
            if (context.Request.RouteValues.TryGetValue(keyName, out var routeVal) &&
                Guid.TryParse(routeVal?.ToString(), out var routeId))
            {
                return routeId;
            }

            return null;
        }
    }
}