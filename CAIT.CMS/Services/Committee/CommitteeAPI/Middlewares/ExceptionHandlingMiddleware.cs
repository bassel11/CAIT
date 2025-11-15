using FluentValidation;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Text.Json;

namespace CommitteeAPI.Middleware // ضع الـ namespace حسب طبقة العرض لديك
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception occurred");

                var response = context.Response;
                response.ContentType = "application/json";

                var statusCode = HttpStatusCode.InternalServerError;
                var message = "An unexpected error occurred.";

                switch (ex)
                {
                    case UnauthorizedAccessException:
                        statusCode = HttpStatusCode.Unauthorized;
                        message = ex.Message;
                        break;

                    case ValidationException:
                        statusCode = HttpStatusCode.UnprocessableEntity;
                        message = ex.Message;
                        break;

                    case KeyNotFoundException:
                        statusCode = HttpStatusCode.NotFound;
                        message = ex.Message;
                        break;

                    case DbUpdateException:
                        statusCode = HttpStatusCode.BadRequest;
                        message = ex.Message;
                        break;

                    default:
                        message = ex.Message;
                        if (ex.InnerException != null)
                            message += "\n" + ex.InnerException.Message;
                        break;
                }

                response.StatusCode = (int)statusCode;

                var result = JsonSerializer.Serialize(new
                {
                    Succeeded = false,
                    Message = message,
                    StatusCode = statusCode
                });

                await response.WriteAsync(result);
            }
        }
    }
}