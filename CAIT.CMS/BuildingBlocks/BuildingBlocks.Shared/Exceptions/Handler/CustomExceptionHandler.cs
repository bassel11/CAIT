using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace BuildingBlocks.Shared.Exceptions.Handler
{
    public class CustomExceptionHandler(ILogger<CustomExceptionHandler> logger) : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception,
            CancellationToken cancellationToken)
        {
            // تسجيل الخطأ بدقة
            logger.LogError(exception, "🔴 Error detected: {Type} | Message: {Message}", exception.GetType().Name, exception.Message);

            // 1. القاموس الشامل لتحويل الاستثناءات إلى Status Codes
            (string Title, string Detail, int StatusCode) details = exception switch
            {
                // --- أخطاء النظام المخصصة (Custom) ---
                InternalServerException => ("Internal Server Error", exception.Message, StatusCodes.Status500InternalServerError),
                ValidationException => ("Validation Failure", "One or more validation errors occurred.", StatusCodes.Status400BadRequest),
                BadRequestException => ("Bad Request", exception.Message, StatusCodes.Status400BadRequest),
                NotFoundException => ("Resource Not Found", exception.Message, StatusCodes.Status404NotFound),
                DomainException => ("Business Rule Violation", exception.Message, StatusCodes.Status400BadRequest),
                IdentityServiceUnavailableException => ("Service Unavailable", "Identity service is down.", StatusCodes.Status503ServiceUnavailable),

                // --- أخطاء .NET القياسية (للمستقبل وللحالات غير المتوقعة) ---
                KeyNotFoundException => ("Not Found", "The requested resource key was not found.", StatusCodes.Status404NotFound),
                UnauthorizedAccessException => ("Unauthorized", "Access is denied.", StatusCodes.Status401Unauthorized),
                ArgumentNullException => ("Invalid Argument", "A required value was null.", StatusCodes.Status400BadRequest),
                ArgumentException => ("Invalid Argument", exception.Message, StatusCodes.Status400BadRequest),
                OperationCanceledException => ("Request Canceled", "The operation was canceled by the client.", 499), // Client Closed Request
                TimeoutException => ("Timeout", "The operation timed out.", StatusCodes.Status504GatewayTimeout),
                FormatException => ("Format Error", "The input format is invalid.", StatusCodes.Status400BadRequest),

                // --- الحالة الافتراضية (Catch-All) ---
                _ => ("Internal Server Error", "An unexpected error occurred. Please contact support.", StatusCodes.Status500InternalServerError)
            };

            // 2. بناء كائن ProblemDetails القياسي (RFC 7807)
            var problemDetails = new ProblemDetails
            {
                Title = details.Title,
                Detail = details.Detail,
                Status = details.StatusCode,
                Instance = context.Request.Path,
                Type = exception.GetType().Name
            };

            // 3. إضافة TraceId (هام جداً للربط مع الـ Logs)
            problemDetails.Extensions.Add("traceId", context.TraceIdentifier);

            // 4. إضافة تفاصيل خاصة حسب نوع الخطأ
            if (exception is ValidationException validationException)
            {
                problemDetails.Extensions.Add("errors", validationException.Errors);
            }

            if (exception is BadRequestException badRequestEx && !string.IsNullOrEmpty(badRequestEx.Details))
            {
                problemDetails.Extensions.Add("reason", badRequestEx.Details);
            }

            // كتابة الاستجابة
            context.Response.StatusCode = details.StatusCode;
            await context.Response.WriteAsJsonAsync(problemDetails, cancellationToken: cancellationToken);

            return true; // تم التعامل مع الخطأ
        }
    }
}