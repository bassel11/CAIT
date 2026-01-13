using BuildingBlocks.Contracts.Audit;
using BuildingBlocks.Shared.Services;
using Identity.Core.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;

namespace Identity.Infrastructure.Interceptors
{
    public class AuditPublishingInterceptor : SaveChangesInterceptor
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ICurrentUserService _currentUser;

        public AuditPublishingInterceptor(IServiceProvider serviceProvider, ICurrentUserService currentUser)
        {
            _serviceProvider = serviceProvider;
            _currentUser = currentUser;
        }

        public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
            DbContextEventData eventData,
            InterceptionResult<int> result,
            CancellationToken cancellationToken = default)
        {
            if (eventData.Context == null) return await base.SavingChangesAsync(eventData, result, cancellationToken);

            await PublishLocalAuditEvents(eventData.Context, cancellationToken);

            return await base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        private async Task PublishLocalAuditEvents(DbContext context, CancellationToken ct)
        {
            // 1. قائمة الأسماء المحظورة (الأسماء المختصرة)
            var ignoredEntities = new HashSet<string>
            {
                "OutboxMessage",
                "OutboxState",
                "InboxState",
                "AuditLog",
                "IntegrationEventLogEntry",
                //"UserRolePermReso",
                "UserPasswordHistory",
                
                // التأكد من كتابة الأسماء كما هي في الكلاسات
                nameof(RefreshToken),
                "IdentityUserToken",
                "UserToken",
                "UserLogin",
                "RoleClaim",
                "UserClaim"
            };

            // 2. جلب الإدخالات المعدلة
            var entries = context.ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified || e.State == EntityState.Deleted)
                .ToList(); // نجلب القائمة أولاً ثم نفلتر بداخل الـ Loop لدقة أعلى

            if (entries.Count == 0) return;

            var publisher = _serviceProvider.GetRequiredService<IPublisher>();

            foreach (var entry in entries)
            {
                // =================================================================
                // 🛑 الفلترة الدقيقة (The Critical Fix)
                // =================================================================

                // 1. الحصول على الاسم المختصر (Class Name Only)
                // مثال: يعيد "RefreshToken" بدلاً من "Identity.Core.Entities.RefreshToken"
                var simpleName = entry.Metadata.ClrType.Name;

                // 2. الحصول على الاسم الكامل (للاحتياط)
                var fullName = entry.Metadata.Name;

                // 3. التحقق:
                // أ) هل الاسم المختصر موجود في القائمة؟
                // ب) هل الاسم الكامل ينتهي بأحد الأسماء المحظورة؟ (حماية إضافية)
                // ج) هل هو DTO أو View؟
                if (ignoredEntities.Contains(simpleName) ||
                    fullName.Contains("RefreshToken") || // 👈 تشرط صريح لمنع الريفرش توكن مهما كان اسمه
                    simpleName.EndsWith("DTO") ||
                    simpleName.Contains("View"))
                {
                    continue; // 🚫 تخطي هذا السجل وعدم إرساله للـ Audit
                }
                // =================================================================

                Dictionary<string, object?> oldVal = new();
                Dictionary<string, object?> newVal = new();
                List<string> changedCols = new();

                foreach (var prop in entry.Properties)
                {
                    string propName = prop.Metadata.Name;
                    if (propName == "DomainEvents") continue;

                    if (entry.State == EntityState.Added)
                    {
                        newVal[propName] = prop.CurrentValue;
                    }
                    else if (entry.State == EntityState.Deleted)
                    {
                        oldVal[propName] = prop.OriginalValue;
                    }
                    else if (entry.State == EntityState.Modified && prop.IsModified)
                    {
                        oldVal[propName] = prop.OriginalValue;
                        newVal[propName] = prop.CurrentValue;
                        changedCols.Add(propName);
                    }
                }

                if (entry.State == EntityState.Modified && changedCols.Count == 0) continue;

                string? committeeId = null;
                var committeeProp = entry.Entity.GetType().GetProperty("CommitteeId");
                if (committeeProp != null)
                {
                    committeeId = committeeProp.GetValue(entry.Entity)?.ToString();
                }

                var localEvent = new AuditLogEvent(
                    EntityName: simpleName, // نستخدم الاسم المختصر هنا ليكون أجمل في اللوج
                    PrimaryKey: entry.Properties.FirstOrDefault(p => p.Metadata.IsPrimaryKey())?.CurrentValue?.ToString() ?? "N/A",
                    ActionType: entry.State.ToString(),
                    UserId: _currentUser.UserId.ToString(),
                    UserName: _currentUser.UserName ?? "Anonymous",
                    Email: _currentUser.Email ?? "NoEmail",
                    CommitteeId: committeeId,
                    Justification: null,
                    Severity: entry.State == EntityState.Deleted ? "Warning" : "Info",
                    OldValues: oldVal.Count > 0 ? JsonSerializer.Serialize(oldVal) : null,
                    NewValues: newVal.Count > 0 ? JsonSerializer.Serialize(newVal) : null,
                    ChangedColumns: changedCols.Count > 0 ? string.Join(",", changedCols) : null
                );

                await publisher.Publish(localEvent, ct);
            }
        }
    }
}