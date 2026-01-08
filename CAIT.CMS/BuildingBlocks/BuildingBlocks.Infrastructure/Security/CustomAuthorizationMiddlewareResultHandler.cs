using BuildingBlocks.Shared.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Net;

namespace BuildingBlocks.Infrastructure.Security
{
    public class CustomAuthorizationMiddlewareResultHandler : IAuthorizationMiddlewareResultHandler
    {
        private readonly AuthorizationMiddlewareResultHandler _defaultHandler = new();

        // Logger Singleton (Safe)
        private readonly ILogger<CustomAuthorizationMiddlewareResultHandler> _logger;

        public CustomAuthorizationMiddlewareResultHandler(ILogger<CustomAuthorizationMiddlewareResultHandler> logger)
        {
            _logger = logger;
        }

        public async Task HandleAsync(
            RequestDelegate next,
            HttpContext context,
            AuthorizationPolicy policy,
            PolicyAuthorizationResult authorizeResult)
        {
            // 1. Success: دع الطلب يمر
            if (authorizeResult.Succeeded)
            {
                await _defaultHandler.HandleAsync(next, context, policy, authorizeResult);
                return;
            }

            // =================================================================
            // 2. Unauthorized (401): المستخدم غير مسجل دخول أو التوكن غير صالح
            // =================================================================
            if (authorizeResult.Challenged)
            {
                // تسجيل معلومة بسيطة (Info) وليست تحذير لأن انتهاء التوكن أمر طبيعي
                _logger.LogInformation("⚠️ Authentication Failed (401). Token might be missing, invalid, or expired.");

                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                context.Response.ContentType = "application/json";

                // إضافة الهيدر القياسي (مهم لبعض المتصفحات)
                context.Response.Headers.Append("WWW-Authenticate", "Bearer");

                var response = new
                {
                    StatusCode = 401,
                    Error = "Unauthorized",
                    Message = "Authentication failed. You are not logged in or your token has expired.",
                    Hint = "Please check your 'Authorization' header (Bearer Token) or refresh your token."
                };

                await context.Response.WriteAsJsonAsync(response);
                return;
            }

            // =================================================================
            // 3. Forbidden (403): المستخدم مسجل دخول لكن لا يملك صلاحية
            // =================================================================
            if (authorizeResult.Forbidden)
            {
                // استخراج أسماء الصلاحيات الناقصة
                var requiredPermissions = policy.Requirements
                    .OfType<PermissionRequirement>()
                    .Select(r => r.PermissionName)
                    .ToList();

                // تسجيل تحذير (Warning) لأن هذه محاولة وصول لموارد غير مسموحة
                var userId = context.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "Unknown";
                _logger.LogWarning("⛔ Access Denied (403). User: {UserId}. Missing Permissions: {Permissions}",
                    userId, string.Join(", ", requiredPermissions));

                context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                context.Response.ContentType = "application/json";

                var response = new
                {
                    StatusCode = 403,
                    Error = "Forbidden",
                    Message = "You do not have the required permissions to access this resource.",
                    RequiredPermissions = requiredPermissions,
                    Hint = "Please ensure you have the correct permission (Global, Resource-Specific, or Parent-Inherited)."
                };

                await context.Response.WriteAsJsonAsync(response);
                return;
            }

            // Fallback (أي حالة أخرى نادرة)
            await _defaultHandler.HandleAsync(next, context, policy, authorizeResult);
        }
    }
}