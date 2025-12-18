using Audit.Application.Contracts;
using Audit.Application.Repositories;
using Audit.Application.Services;
using Audit.Infrastructure.Consumers;
using Audit.Infrastructure.Data;
using Audit.Infrastructure.Repositories;
using Audit.Infrastructure.Services;
using BuildingBlocks.Infrastructure;
using BuildingBlocks.Messaging.MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Audit.Infrastructure
{
    public static class InfrastructureDependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // =========================================================
            // 1. تسجيل الخدمات المشتركة (Shared BuildingBlocks)
            // =========================================================

            // تسجيل المستخدم والـ HttpContext
            services.AddSharedInfrastructure();

            // تسجيل نظام التصاريح (Policy Provider + Handler)
            services.AddDynamicPermissions();

            // تسجيل خدمة الاتصال بالهوية (Identity Service Client)
            // بدلاً من كتابة كود HttpClient و DelegatingHandler يدوياً
            var identityUrl = configuration["Services:IdentityBaseUrl"];
            if (!string.IsNullOrEmpty(identityUrl))
            {
                services.AddRemotePermissionService(identityUrl);
            }
            else
            {
                // تحذير أو استثناء إذا كان الرابط مفقوداً
                throw new InvalidOperationException("IdentityBaseUrl is not configured in appsettings.");
            }

            // =========================================================
            // 2. إعدادات Audit الخاصة (Database & Repos)
            // =========================================================

            var connectionString = configuration.GetConnectionString("AuditConnectionString");
            services.AddDbContext<AuditDbContext>(options =>
            {
                options.UseSqlServer(connectionString);
            });

            // Repositories & Services الخاصة بالتدقيق فقط
            services.AddScoped<IAuditStore, AuditStore>();
            services.AddScoped<IAuditReadRepository, AuditReadRepository>();
            services.AddScoped<IAuditQueryNewService, AuditQueryNewService>();

            // (تم حذف JwtDelegatingHandler من هنا لأنه أصبح ضمن الامتداد المشترك)

            // =========================================================
            // 3. MassTransit
            // =========================================================
            services.AddMessageBroker<AuditDbContext>(
                configuration,
                typeof(AuditLogCreatedConsumer).Assembly
            );

            return services;
        }
    }
}