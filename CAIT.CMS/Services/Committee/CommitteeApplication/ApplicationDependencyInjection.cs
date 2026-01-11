using BuildingBlocks.Shared.Behaviors;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace CommitteeApplication
{
    public static class ApplicationDependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // تحديد الـ Assembly الحالي مرة واحدة لاستخدامه
            var assembly = Assembly.GetExecutingAssembly();

            // 1. FluentValidation
            // تسجيل كل الـ Validators تلقائياً
            services.AddValidatorsFromAssembly(assembly);

            // 2. AutoMapper
            // تسجيل كل الـ Profiles تلقائياً
            services.AddAutoMapper(assembly);

            // 3. MediatR & Pipeline Behaviors
            services.AddMediatR(cfg =>
            {
                // تسجيل الـ Handlers (Commands/Queries)
                cfg.RegisterServicesFromAssembly(assembly);

                // ✅ تسجيل السلوكيات المشتركة (من BuildingBlocks)

                // أ) التحقق (Validation): يرمي ValidationException الموحد (يمنع دخول البيانات الخاطئة للـ Handler)
                cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));

                // ب) التسجيل (Logging): يسجل تفاصيل الطلب والأداء (اختياري ولكنه مفضل)
                cfg.AddOpenBehavior(typeof(LoggingBehavior<,>));

                // ج) التعامل مع الاستثناءات غير المعالجة (إذا كنت تستخدمه كـ Behavior وليس Middleware)
                // cfg.AddOpenBehavior(typeof(UnhandledExceptionBehavior<,>));
            });

            // ❌ تم الحذف: services.AddScoped<ICurrentUserService, CurrentUserService>();
            // السبب: يجب نقل تسجيل هذه الخدمة إلى CommitteeInfrastructure

            return services;

            return services;
        }
    }
}
