using BuildingBlocks.Contracts.SecurityEvents;
using Identity.Core.Entities;
using MassTransit;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;

namespace Identity.Infrastructure.Interceptors
{
    public class PermissionChangeInterceptor : SaveChangesInterceptor
    {
        // نستخدم ServiceProvider لكسر دائرة الاعتماد (Lazy Resolution)
        private readonly IServiceProvider _serviceProvider;

        // خاصية للتحكم في النشر (إيقاف/تشغيل) لمنع المشاكل أثناء الـ Seeding
        public bool SuppressPublishing { get; set; } = false;

        public PermissionChangeInterceptor(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
            DbContextEventData eventData,
            InterceptionResult<int> result,
            CancellationToken cancellationToken = default)
        {
            // 1. إذا تم تفعيل الإيقاف، لا تفعل شيئاً (لأغراض الـ Seeding)
            if (SuppressPublishing)
            {
                return await base.SavingChangesAsync(eventData, result, cancellationToken);
            }

            var context = eventData.Context;
            if (context == null) return await base.SavingChangesAsync(eventData, result, cancellationToken);

            // تجميع المعرفات المتأثرة (HashSet لمنع التكرار)
            var affectedUserIds = new HashSet<Guid>();

            // 2. مراقبة التغييرات
            foreach (var entry in context.ChangeTracker.Entries())
            {
                // أ) تغيير حالة المستخدم (تجميد/تفعيل)
                if (entry.Entity is ApplicationUser user && entry.State == EntityState.Modified)
                {
                    // نتحقق إذا كان التعديل في عمود IsActive تحديداً
                    var isActiveProp = entry.Property(nameof(ApplicationUser.IsActive));
                    if (isActiveProp.IsModified)
                    {
                        affectedUserIds.Add(user.Id);
                    }
                }

                // ب) الصلاحيات المخصصة (إضافة، تعديل، حذف)
                if (entry.Entity is UserRolePermReso customPerm &&
                   (entry.State == EntityState.Added || entry.State == EntityState.Modified || entry.State == EntityState.Deleted))
                {
                    affectedUserIds.Add(customPerm.UserId);
                }

                // ج) أدوار المستخدم (إضافة دور، سحب دور)
                if (entry.Entity is IdentityUserRole<Guid> userRole &&
                   (entry.State == EntityState.Added || entry.State == EntityState.Deleted))
                {
                    affectedUserIds.Add(userRole.UserId);
                }
            }

            // 3. النشر الآمن (Lazy Loading)
            if (affectedUserIds.Count > 0)
            {
                // 🔥 نطلب الناشر هنا فقط (بعد اكتمال بناء DbContext) لتجنب توقف التطبيق
                var publishEndpoint = _serviceProvider.GetRequiredService<IPublishEndpoint>();

                foreach (var userId in affectedUserIds)
                {
                    // سيتم حفظ الرسالة في جدول OutboxMessage ضمن نفس الـ Transaction
                    await publishEndpoint.Publish(new UserPermissionsChangedIntegrationEvent
                    {
                        UserId = userId,
                        OccurredAt = DateTime.UtcNow
                    }, cancellationToken);
                }
            }

            return await base.SavingChangesAsync(eventData, result, cancellationToken);
        }
    }
}
