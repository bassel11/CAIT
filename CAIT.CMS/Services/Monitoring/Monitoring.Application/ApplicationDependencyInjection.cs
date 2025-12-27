using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Monitoring.Application
{
    public static class ApplicationDependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            return services;
        }
    }
}
