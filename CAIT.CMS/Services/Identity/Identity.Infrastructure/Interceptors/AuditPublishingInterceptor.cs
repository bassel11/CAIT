using BuildingBlocks.Shared.Services;
using Identity.Core.Events.Audit; // تأكد من الـ Namespace الصحيح
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection; // ضروري جداً
using System.Text.Json;

namespace Identity.Infrastructure.Interceptors
{
    public class AuditPublishingInterceptor : SaveChangesInterceptor
    {
        // التغيير هنا:  ServiceProvider بدلاً من Publisher مباشرة
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
            // 1. فحص هل هناك تغييرات تستحق النشر؟
            var entries = context.ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified || e.State == EntityState.Deleted)
                .Where(e => !e.Metadata.Name.Contains("Outbox") &&
                            !e.Metadata.Name.Contains("Audit") &&
                            !e.Metadata.Name.Contains("IntegrationEvent"))
                .ToList();

            if (entries.Count == 0) return;

            // 👇 الحل السحري: نطلب الناشر الآن فقط (عند الحاجة) وليس عند تشغيل التطبيق
            // هذا يمنع مشكلة الـ Circular Dependency
            // using var scope = _serviceProvider.CreateScope();
            var publisher = _serviceProvider.GetRequiredService<IPublisher>();

            foreach (var entry in entries)
            {
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

                // استخراج CommitteeId إن وجد (أو أي معرف آخر)
                string? committeeId = null;
                var committeeProp = entry.Entity.GetType().GetProperty("CommitteeId");
                if (committeeProp != null)
                {
                    committeeId = committeeProp.GetValue(entry.Entity)?.ToString();
                }

                // إنشاء الحدث
                var localEvent = new AuditLogEvent(
                    EntityName: entry.Entity.GetType().Name,
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

                // النشر
                await publisher.Publish(localEvent, ct);
            }
        }
    }
}