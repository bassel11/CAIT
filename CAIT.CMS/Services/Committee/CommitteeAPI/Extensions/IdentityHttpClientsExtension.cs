using CommitteeApplication.Interfaces.Authorization;
using CommitteeApplication.Interfaces.Roles;
using CommitteeInfrastructure.Authorization;
using CommitteeInfrastructure.Roles;

namespace CommitteeAPI.Extensions
{
    public static class IdentityHttpClientsExtension
    {
        public static IServiceCollection AddIdentityHttpClients(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var baseUrlString = configuration["Services:IdentityBaseUrl"] ?? throw new InvalidOperationException("IdentityBaseUrl is not configured");
            var baseUrl = new Uri(baseUrlString);


            services.AddHttpClient<IRoleServiceHttpClient, RoleServiceHttpClient>(client =>
            {
                client.BaseAddress = baseUrl;
            });

            services.AddHttpClient<IPermissionService, PermissionServiceHttpClient>(client =>
            {
                client.BaseAddress = baseUrl;
            });

            return services;
        }
    }
}
