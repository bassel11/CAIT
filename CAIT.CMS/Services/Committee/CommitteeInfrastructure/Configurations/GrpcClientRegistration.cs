using CommitteeApplication.Interfaces.Grpc;
using CommitteeInfrastructure.Grpc.Clients;
using Identity.GrpcContracts;
using Microsoft.Extensions.DependencyInjection;

namespace CommitteeInfrastructure.Configurations
{
    public static class GrpcClientRegistration
    {
        public static IServiceCollection AddIdentityGrpcClient(this IServiceCollection services, string identityGrpcUrl)
        {
            services.AddGrpcClient<IdentityUserService.IdentityUserServiceClient>(o =>
            {
                o.Address = new Uri(identityGrpcUrl);
            });

            services.AddScoped<IUserGrpcService, UserGrpcService>();
            return services;
        }
    }
}
