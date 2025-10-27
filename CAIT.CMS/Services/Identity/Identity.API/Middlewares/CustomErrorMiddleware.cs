using Identity.API.Models;
using System.Text.Json;

namespace Identity.API.Middlewares
{
    public class CustomErrorMiddleware
    {
        private readonly RequestDelegate _next;

        public CustomErrorMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            await _next(context);

            if (context.Response.StatusCode == 403)
            {
                context.Response.ContentType = "application/json";
                var error = new ErrorResponse
                {
                    StatusCode = 403,
                    Error = "Access Denied",
                    Message = "You do not have permission to access this resource."
                };
                await context.Response.WriteAsync(JsonSerializer.Serialize(error));
            }
        }
    }
}
