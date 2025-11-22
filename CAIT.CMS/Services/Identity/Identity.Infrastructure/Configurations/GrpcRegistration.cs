using Identity.Application.Interfaces.Grpc;
using Identity.Infrastructure.Grpc.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Identity.Infrastructure.Configurations
{
    public static class GrpcRegistration
    {
        public static IServiceCollection AddIdentityGrpc(this IServiceCollection services)
        {
            services.AddScoped<IUserGrpcService, UserGrpcServiceImpl>();
            services.AddGrpc();
            services.AddScoped<UserGrpcService>();
            return services;
        }
    }
}
