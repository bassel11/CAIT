using Asp.Versioning;
using BuildingBlocks.Infrastructure.Security; // تأكد من وجود هذا الـ Namespace للكلاسات الجديدة
using BuildingBlocks.Infrastructure.Services;
using BuildingBlocks.Infrastructure.Swagger;
using BuildingBlocks.Shared.Authorization;
using BuildingBlocks.Shared.Exceptions.Handler;
using BuildingBlocks.Shared.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace BuildingBlocks.Infrastructure
{
    public static class InfrastructureServiceExtensions
    {
        public static IServiceCollection AddSharedInfrastructure(this IServiceCollection services)
        {
            services.AddHttpContextAccessor();
            services.AddScoped<ICurrentUserService, CurrentUserService>();
            services.AddScoped<IDateTimeProvider, DateTimeProvider>();
            return services;
        }
        public static IServiceCollection AddDynamicPermissions(this IServiceCollection services)
        {
            services.AddSingleton<IAuthorizationPolicyProvider, DynamicAuthorizationPolicyProvider>();
            services.AddScoped<IAuthorizationHandler, PermissionHandler>();
            services.AddScoped<ResourceExtractionMiddleware>();
            services.AddTransient<JwtDelegatingHandler>();
            services.AddSingleton<IAuthorizationMiddlewareResultHandler, CustomAuthorizationMiddlewareResultHandler>();
            return services;
        }
        public static IServiceCollection AddRemotePermissionService(
            this IServiceCollection services,
            string identityBaseUrl,
            IConfiguration configuration,
            string serviceName)
        {

            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = configuration.GetConnectionString("Redis");
                options.InstanceName = $"cms:{serviceName}";
            });

            if (string.IsNullOrWhiteSpace(identityBaseUrl))
                throw new ArgumentNullException(nameof(identityBaseUrl), "Identity Service URL cannot be empty.");

            services.AddHttpClient<IHttpPermissionFetcher, HttpPermissionFetcher>(client =>
            {
                client.BaseAddress = new Uri(identityBaseUrl!);
            })
             .AddHttpMessageHandler<JwtDelegatingHandler>();

            services.AddScoped<IPermissionService>(sp =>
            {
                var cache = sp.GetRequiredService<Microsoft.Extensions.Caching.Distributed.IDistributedCache>();
                var fetcher = sp.GetRequiredService<IHttpPermissionFetcher>();
                return new RedisPermissionService(cache, fetcher, serviceName);
            });

            return services;
        }

        public static IApplicationBuilder UsePermissionMiddleware(this IApplicationBuilder app)
        {
            app.UseMiddleware<ResourceExtractionMiddleware>();
            return app;
        }

        public static IServiceCollection AddCoreInfrastructure(this IServiceCollection services)
        {
            services.AddExceptionHandler<CustomExceptionHandler>();
            services.AddProblemDetails();
            return services;
        }

        public static IServiceCollection AddEnterpriseVersioning(
                    this IServiceCollection services,
                    string apiTitle,
                    string apiDescription)
        {
            // 1. تسجيل الإعدادات كـ Singleton ليتم حقنها في ConfigureSwaggerOptions
            var settings = new SwaggerApiSettings
            {
                Title = apiTitle,
                Description = apiDescription
            };
            services.AddSingleton(settings);

            // 2. إعداد Versioning (كما هو)
            var builder = services.AddApiVersioning(options =>
            {
                options.ReportApiVersions = true;
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.ApiVersionReader = new UrlSegmentApiVersionReader();
            });

            builder.AddApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
            });

            builder.AddMvc();

            // 3. تسجيل ConfigureSwaggerOptions (الذي سيستخدم settings أعلاه)
            services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();

            return services;
        }
    }
}