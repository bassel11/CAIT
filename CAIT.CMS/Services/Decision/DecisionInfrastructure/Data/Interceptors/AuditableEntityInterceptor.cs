using BuildingBlocks.Shared.Services; // ✅ ضروري
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace DecisionInfrastructure.Data.Interceptors
{
    public class AuditableEntityInterceptor : SaveChangesInterceptor
    {
        private readonly ICurrentUserService _currentUser;

        // ✅ حقن خدمة المستخدم
        public AuditableEntityInterceptor(ICurrentUserService currentUser)
        {
            _currentUser = currentUser;
        }

        public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
        {
            UpdateEntities(eventData.Context);
            return base.SavingChanges(eventData, result);
        }

        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
        {
            UpdateEntities(eventData.Context);
            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        private void UpdateEntities(DbContext? context)
        {
            if (context == null) return;

            // ✅ استخدام ICurrentUserService للحصول على المعرف
            // نستخدم UserId لأنه ثابت، بينما الاسم قد يتغير.
            // إذا كنت تفضل تخزين الاسم، استبدل .UserId.ToString() بـ .UserName
            var userId = _currentUser.UserId != Guid.Empty ? _currentUser.UserId.ToString() : "System";
            var now = DateTime.UtcNow; // ✅ استخدام UtcNow كمعيار قياسي

            foreach (var entry in context.ChangeTracker.Entries<IEntity>())
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.CreatedBy = userId;
                    entry.Entity.CreatedAt = now;
                }

                if (entry.State == EntityState.Added || entry.State == EntityState.Modified)
                {
                    entry.Entity.LastModifiedBy = userId;
                    entry.Entity.LastTimeModified = now;
                }
            }
        }
    }
}

// يفضل نقل Extension Method لملف منفصل، لكن يمكن إبقاؤها هنا
public static class Extensions
{
    public static bool HasChangedOwnedEntities(this EntityEntry entry) =>
        entry.References.Any(r =>
            r.TargetEntry != null &&
            r.TargetEntry.Metadata.IsOwned() &&
            (r.TargetEntry.State == EntityState.Added || r.TargetEntry.State == EntityState.Modified));
}