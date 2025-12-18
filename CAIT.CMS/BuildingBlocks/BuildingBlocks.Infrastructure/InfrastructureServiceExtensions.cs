using BuildingBlocks.Infrastructure.Security; // تأكد من وجود هذا الـ Namespace للكلاسات الجديدة
using BuildingBlocks.Infrastructure.Services;
using BuildingBlocks.Shared.Authorization;
using BuildingBlocks.Shared.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
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
        public static IServiceCollection AddRemotePermissionService(this IServiceCollection services, string identityBaseUrl)
        {
            if (string.IsNullOrWhiteSpace(identityBaseUrl))
                throw new ArgumentNullException(nameof(identityBaseUrl), "Identity Service URL cannot be empty.");

            // تسجيل HttpClient مع تفعيل الـ Handler لإرفاق التوكن تلقائياً
            services.AddHttpClient<IPermissionService, PermissionServiceHttpClient>(client =>
            {
                client.BaseAddress = new Uri(identityBaseUrl);
            })
            .AddHttpMessageHandler<JwtDelegatingHandler>();

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