using BuildingBlocks.Infrastructure;
using BuildingBlocks.Infrastructure.Security;
using BuildingBlocks.Messaging.MassTransit;
using CommitteeApplication.Interfaces.Roles;
using CommitteeApplication.Wrappers;
using CommitteeCore.Repositories;
using CommitteeInfrastructure.Data;
using CommitteeInfrastructure.Interceptors;
using CommitteeInfrastructure.Repositories;
using CommitteeInfrastructure.Roles;
using CommitteeInfrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CommitteeInfrastructure
{
    public static class InfrastructureDependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(
              this IServiceCollection services
            , IConfiguration configuration)
        {

            // 1. تسجيل البنية التحتية الأساسية (User, HttpContext)
            services.AddSharedInfrastructure();

            // 2. تسجيل نظام التصاريح (الجديد)
            services.AddDynamicPermissions();

            // 3. تسجيل خدمة الاتصال بالهوية (للتحقق من الصلاحيات)
            // يجب أن تتأكد من وجود هذا الرابط في appsettings.json
            //var identityUrl = configuration["Services:IdentityBaseUrl"]
            //    ?? throw new InvalidOperationException("IdentityBaseUrl is not configured");

            //if (!string.IsNullOrEmpty(identityUrl))
            //{
            //    //services.AddRemotePermissionService(identityUrl);
            //}

            var identityUrl = configuration["Services:IdentityBaseUrl"];
            if (!string.IsNullOrEmpty(identityUrl))
            {
                services.AddRemotePermissionService(
                    identityUrl,
                    configuration: configuration,
                    serviceName: "committee:");
            }

            // Role client
            services.AddHttpClient<IRoleServiceHttpClient, RoleServiceHttpClient>(client =>
            {
                client.BaseAddress = new Uri(identityUrl);
            })
            .AddHttpMessageHandler<JwtDelegatingHandler>();

            services.AddScoped<AuditPublishingInterceptor>();

            var connectionString = configuration.GetConnectionString("CommitteeConnectionString");
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException("CRITICAL: Connection string 'CommitteeConnectionString' is missing.");
            }
            services.AddDbContext<CommitteeContext>((sp, options) =>
            {
                // Resolve the interceptor from the service provider
                var auditInterceptor = sp.GetRequiredService<AuditPublishingInterceptor>();

                options.UseSqlServer(connectionString)
                       .AddInterceptors(auditInterceptor); // Add interceptor to DbContext options
            });

            // Repositories
            services.AddScoped(typeof(IAsyncRepository<>), typeof(RepositoryBase<>));
            services.AddScoped<ICommitteeRepository, CommitteeRepository>();
            services.AddScoped<ICommitteeMemberRepository, CommitteeMemberRepository>();
            services.AddScoped<ICommitteeMemberRoleRepository, CommitteeMemberRoleRepository>();
            services.AddScoped<IStatusHistoryRepository, StatusHistoryRepository>();
            services.AddScoped<ICommitteeQuorumRuleRepository, CommitteeQuorumRuleRepository>();

            // Services
            services.AddScoped<IPaginationService, PaginationService>();

            services.AddMessageBroker<CommitteeContext>(
                configuration,
                typeof(CommitteeApplication.ApplicationDependencyInjection).Assembly
            );

            return services;
        }
    }
}
