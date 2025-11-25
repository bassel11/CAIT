using MeetingApplication.Interfaces.Authorization;
using MeetingInfrastructure.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MeetingInfrastructure.Integrations.Identity
{
    public static class IdentityServiceExtensions
    {
        public static IServiceCollection AddIdentityServiceClient(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var baseUrlString = configuration["Services:IdentityBaseUrl"]
                ?? throw new InvalidOperationException("IdentityBaseUrl is not configured");

            var baseUrl = new Uri(baseUrlString);

            // Permission client
            services.AddHttpClient<IPermissionService, PermissionServiceHttpClient>(client =>
            {
                client.BaseAddress = baseUrl;
            })
            .AddHttpMessageHandler<JwtDelegatingHandler>();   // إضافة التوكن

            // Role client
            //services.AddHttpClient<IRoleServiceHttpClient, RoleServiceHttpClient>(client =>
            //{
            //    client.BaseAddress = baseUrl;
            //})
            //.AddHttpMessageHandler<JwtDelegatingHandler>();   // إضافة التوكن



            return services;
        }
    }
}
