using Audit.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Audit.Application
{
    public static class ApplicationDependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<IAuditQueryService, AuditQueryService>();

            return services;
        }
    }
}
