using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BuildingBlocks.Shared.Exceptions.Handler
{
    public class CustomExceptionHandler(ILogger<CustomExceptionHandler> logger) : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception,
            CancellationToken cancellationToken)
        {
            // 1. Log the full error on the server (So YOU can see the stack trace in the console/files)
            logger.LogError(exception, "🔴 Error detected: {Type} | Message: {Message}", exception.GetType().Name, exception.Message);

            // 2. Determine response details
            (string Title, string Detail, int StatusCode) details = exception switch
            {
                DbUpdateException dbEx => HandleDatabaseException(dbEx),
                ValidationException => ("Validation Failure", "One or more validation errors occurred.", StatusCodes.Status400BadRequest),
                BadRequestException => ("Bad Request", exception.Message, StatusCodes.Status400BadRequest),
                NotFoundException => ("Resource Not Found", exception.Message, StatusCodes.Status404NotFound),
                DomainException => ("Business Rule Violation", exception.Message, StatusCodes.Status400BadRequest),
                IdentityServiceUnavailableException => ("Service Unavailable", "Identity service is down.", StatusCodes.Status503ServiceUnavailable),
                InternalServerException => ("Internal Server Error", exception.Message, StatusCodes.Status500InternalServerError),

                // Standard .NET Exceptions
                KeyNotFoundException => ("Not Found", "The requested resource key was not found.", StatusCodes.Status404NotFound),
                UnauthorizedAccessException => ("Unauthorized", "Access is denied.", StatusCodes.Status401Unauthorized),

                // Catch-All: Show the real message, but keeping it clean
                _ => ("Unexpected Error", exception.Message, StatusCodes.Status500InternalServerError)
            };

            // 3. Build ProblemDetails
            var problemDetails = new ProblemDetails
            {
                Title = details.Title,
                Detail = details.Detail,
                Status = details.StatusCode,
                Instance = context.Request.Path,
                Type = exception.GetType().Name
            };

            // 4. Add Extensions (Only the useful ones)
            problemDetails.Extensions.Add("traceId", context.TraceIdentifier);

            // ❌❌ قمنا بحذف هذا السطر لإزالة الرسالة الطويلة غير المحبذة ❌❌
            // problemDetails.Extensions.Add("stackTrace", exception.StackTrace);

            // ✅ نبقي على InnerException لأنه مفيد وقصير (يخبرك بسبب خطأ SQL)
            if (exception.InnerException != null)
            {
                // نأخذ الرسالة فقط وليس الـ StackTrace الخاص بها
                problemDetails.Extensions.Add("innerException", exception.InnerException.Message);
            }

            if (exception is ValidationException validationException)
            {
                problemDetails.Extensions.Add("errors", validationException.Errors);
            }

            if (exception is BadRequestException badRequestEx && !string.IsNullOrEmpty(badRequestEx.Details))
            {
                problemDetails.Extensions.Add("reason", badRequestEx.Details);
            }

            // 5. Write Response
            context.Response.StatusCode = details.StatusCode;
            await context.Response.WriteAsJsonAsync(problemDetails, cancellationToken: cancellationToken);

            return true;
        }

        private (string Title, string Detail, int StatusCode) HandleDatabaseException(DbUpdateException ex)
        {
            if (ex.InnerException is SqlException sqlEx)
            {
                return sqlEx.Number switch
                {
                    547 => ("Data Conflict (Foreign Key)", $"Referential integrity violation: {sqlEx.Message}", StatusCodes.Status409Conflict),
                    2601 or 2627 => ("Duplicate Entry", $"Unique constraint violation: {sqlEx.Message}", StatusCodes.Status409Conflict),
                    _ => ("Database Error", sqlEx.Message, StatusCodes.Status400BadRequest)
                };
            }

            return ("Database Update Error", ex.InnerException?.Message ?? ex.Message, StatusCodes.Status400BadRequest);
        }
    }
}