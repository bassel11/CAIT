using BuildingBlocks.Infrastructure.Security;
using CommitteeApplication.Interfaces.Roles;
using CommitteeInfrastructure.Roles;

namespace CommitteeAPI.Extensions
{
    public static class IdentityHttpClientsExtension
    {
        public static IServiceCollection AddIdentityHttpClients(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var baseUrlString = configuration["Services:IdentityBaseUrl"]
                ?? throw new InvalidOperationException("IdentityBaseUrl is not configured");

            var baseUrl = new Uri(baseUrlString);

            // Role client
            services.AddHttpClient<IRoleServiceHttpClient, RoleServiceHttpClient>(client =>
            {
                client.BaseAddress = baseUrl;
            })
            .AddHttpMessageHandler<JwtDelegatingHandler>();   // إضافة التوكن

            // Permission client
            //services.AddHttpClient<IPermissionService, PermissionServiceHttpClient>(client =>
            //{
            //    client.BaseAddress = baseUrl;
            //})
            //.AddHttpMessageHandler<JwtDelegatingHandler>();   // إضافة التوكن

            return services;
        }
    }
}
