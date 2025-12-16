using BuildingBlocks.Shared.Exceptions.Handler;

namespace DecisionAPI
{
    public static class ApiDependencyInjection
    {
        public static IServiceCollection AddApiServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddExceptionHandler<CustomExceptionHandler>();
            services.AddHealthChecks();
            return services;
        }

        public static WebApplication UseApiServices(this WebApplication app)
        {
            app.UseExceptionHandler(options => { });
            app.UseHealthChecks("/health");
            return app;
        }

    }
}
