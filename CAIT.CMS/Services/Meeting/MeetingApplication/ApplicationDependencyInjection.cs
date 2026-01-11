using BuildingBlocks.Shared.Behaviors;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace MeetingApplication
{
    public static class ApplicationDependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {

            // نأخذ الـ Assembly الحالي مرة واحدة لاستخدامه في التسجيل
            var assembly = Assembly.GetExecutingAssembly();


            // 1. FluentValidation
            // تسجيل كل الـ Validators الموجودة في هذا المشروع تلقائياً
            services.AddValidatorsFromAssembly(assembly);

            // 2. AutoMapper
            // نسجل الـ Profiles الموجودة في هذا الـ Assembly
            services.AddAutoMapper(assembly);

            // 3. MediatR & Pipeline Behaviors
            services.AddMediatR(cfg =>
            {
                // تسجيل الـ Handlers (Commands/Queries)
                cfg.RegisterServicesFromAssembly(assembly);

                // ✅ تسجيل السلوكيات المشتركة (من BuildingBlocks)

                // أ) التحقق (Validation): يرمي ValidationException الموحد
                cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));

                // ب) التسجيل (Logging): يسجل الطلبات والأخطاء والأداء
                cfg.AddOpenBehavior(typeof(LoggingBehavior<,>));

                // ج) المعاملات (Transaction): 
                // إذا كان TransactionBehavior موجوداً في BuildingBlocks نستخدمه،
                // وإلا إذا كان محلياً في هذا المشروع نستخدم المحلي.
                // يفضل نقله لـ BuildingBlocks إذا كان عاماً.
                // cfg.AddOpenBehavior(typeof(TransactionBehavior<,>)); 
            });

            // ❌ تم الحذف: services.AddScoped<ICurrentUserService, CurrentUserService>();
            // السبب: تنفيذ هذه الخدمة يعتمد على HttpContext وهو جزء من Infrastructure.
            // يجب نقل هذا السطر إلى MeetingInfrastructure/DependencyInjection.cs

            return services;
        }
    }
}
