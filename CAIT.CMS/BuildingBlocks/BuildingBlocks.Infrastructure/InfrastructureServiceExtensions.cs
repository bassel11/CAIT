using BuildingBlocks.Infrastructure.Services;
using BuildingBlocks.Shared.Services;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.Infrastructure
{
    public static class InfrastructureServiceExtensions
    {
        public static IServiceCollection AddSharedInfrastructure(this IServiceCollection services)
        {
            // ضروري لعمل الخدمة
            services.AddHttpContextAccessor();

            // تسجيل خدمة المستخدم
            services.AddScoped<ICurrentUserService, CurrentUserService>();

            return services;
        }
    }
}
