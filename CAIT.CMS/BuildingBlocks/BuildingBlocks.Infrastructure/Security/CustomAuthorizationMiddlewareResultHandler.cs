using BuildingBlocks.Shared.Authorization; // تأكد من وجود PermissionRequirement هنا
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc; // هام جداً لـ ProblemDetails
using Microsoft.Extensions.Logging;
using System.Net;

namespace BuildingBlocks.Infrastructure.Security
{
    public class CustomAuthorizationMiddlewareResultHandler : IAuthorizationMiddlewareResultHandler
    {
        private readonly AuthorizationMiddlewareResultHandler _defaultHandler = new();
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
            // الحالة 1: نجاح (مسموح له بالمرور)
            if (authorizeResult.Succeeded)
            {
                await _defaultHandler.HandleAsync(next, context, policy, authorizeResult);
                return;
            }

            // الحالة 2: غير مسجل دخول (401 Unauthorized)
            if (authorizeResult.Challenged)
            {
                _logger.LogInformation("⚠️ Authentication Failed (401).");
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                context.Response.Headers.Append("WWW-Authenticate", "Bearer");

                var problemDetails = new ProblemDetails
                {
                    Status = StatusCodes.Status401Unauthorized,
                    Title = "Unauthorized",
                    Detail = "Authentication failed. Token is missing, invalid, or expired.",
                    Instance = context.Request.Path,
                    Type = "AuthenticationFailure"
                };

                problemDetails.Extensions.Add("traceId", context.TraceIdentifier);

                await context.Response.WriteAsJsonAsync(problemDetails);
                return;
            }

            // الحالة 3: مسجل دخول لكن ممنوع (403 Forbidden)
            if (authorizeResult.Forbidden)
            {
                var requiredPermissions = policy.Requirements
                    .OfType<PermissionRequirement>()
                    .Select(r => r.PermissionName)
                    .ToList();

                var userId = context.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "Unknown";
                _logger.LogWarning("⛔ Access Denied (403). User: {UserId}. Missing Permissions: {Permissions}", userId, string.Join(", ", requiredPermissions));

                context.Response.StatusCode = (int)HttpStatusCode.Forbidden;

                var problemDetails = new ProblemDetails
                {
                    Status = StatusCodes.Status403Forbidden,
                    Title = "Forbidden",
                    Detail = "You do not have the required permissions to access this resource.",
                    Instance = context.Request.Path,
                    Type = "AuthorizationFailure"
                };

                problemDetails.Extensions.Add("traceId", context.TraceIdentifier);
                problemDetails.Extensions.Add("requiredPermissions", requiredPermissions); // مفيد للـ UI

                await context.Response.WriteAsJsonAsync(problemDetails);
                return;
            }

            // الحالة الافتراضية
            await _defaultHandler.HandleAsync(next, context, policy, authorizeResult);
        }
    }
}