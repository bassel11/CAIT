using BuildingBlocks.Infrastructure.Security; // تأكد من وجود هذا الـ Namespace للكلاسات الجديدة
using BuildingBlocks.Infrastructure.Services;
using BuildingBlocks.Shared.Authorization;
using BuildingBlocks.Shared.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.Infrastructure
{
    public static class InfrastructureServiceExtensions
    {
        /// <summary>
        /// تسجيل الخدمات الأساسية المشتركة (User, HttpContext)
        /// هذا ما كان لديك سابقاً
        /// </summary>
        public static IServiceCollection AddSharedInfrastructure(this IServiceCollection services)
        {
            // 1. ضروري لعمل الخدمة والوصول للـ Headers
            services.AddHttpContextAccessor();

            // 2. تسجيل خدمة المستخدم (User Identity)
            services.AddScoped<ICurrentUserService, CurrentUserService>();

            return services;
        }

        /// <summary>
        /// تسجيل نظام التصاريح الديناميكي (بدون إعدادات JWT)
        /// يضيف الـ PolicyProvider, Handler, Middleware
        /// </summary>
        public static IServiceCollection AddDynamicPermissions(this IServiceCollection services)
        {
            // 1. استبدال مزود السياسات الافتراضي بالمزود الديناميكي الخاص بنا
            services.AddSingleton<IAuthorizationPolicyProvider, DynamicAuthorizationPolicyProvider>();

            // 2. تسجيل الـ Handler الذي يفحص الصلاحية
            services.AddScoped<IAuthorizationHandler, PermissionHandler>();

            // 3. تسجيل الميدل وير لاستخراج ResourceId (يستخدم لاحقاً في UsePermissionMiddleware)
            services.AddScoped<ResourceExtractionMiddleware>();

            // 4. تسجيل معالج الـ Token لطلبات HTTP الصادرة
            services.AddTransient<JwtDelegatingHandler>();

            return services;
        }

        /// <summary>
        /// تسجيل عميل HTTP للاتصال بخدمة الهوية والتحقق من الصلاحيات
        /// (يستخدم في Audit, Decision, Meeting)
        /// </summary>
        public static IServiceCollection AddRemotePermissionService(
            this IServiceCollection services,
            string identityBaseUrl,
            IConfiguration configuration,
            string serviceName)
        {

            // 1. تسجيل Redis مع InstanceName (هذا هو العزل)
            // أي مفتاح تخزنه هذه الخدمة سيبدأ بـ "CMS_Task_" مثلاً
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

            // 3. تسجيل خدمة IPermissionService كـ Scoped
            // هنا نقوم بحقن اسم الخدمة يدوياً للكلاس
            services.AddScoped<IPermissionService>(sp =>
            {
                var cache = sp.GetRequiredService<Microsoft.Extensions.Caching.Distributed.IDistributedCache>();
                var fetcher = sp.GetRequiredService<IHttpPermissionFetcher>();

                // نمرر اسم الخدمة للكلاس
                return new RedisPermissionService(cache, fetcher, serviceName);
            });

            return services;
        }

        /// <summary>
        /// تفعيل الميدل وير الخاص باستخراج ResourceId من الـ Request
        /// </summary>
        public static IApplicationBuilder UsePermissionMiddleware(this IApplicationBuilder app)
        {
            app.UseMiddleware<ResourceExtractionMiddleware>();
            return app;
        }
    }
}