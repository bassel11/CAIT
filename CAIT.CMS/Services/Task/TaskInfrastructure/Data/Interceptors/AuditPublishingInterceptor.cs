using BuildingBlocks.Contracts.Audit;
using BuildingBlocks.Shared.Services;
using MediatR;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Text.Json;

namespace TaskInfrastructure.Data.Interceptors
{
    public class AuditPublishingInterceptor : SaveChangesInterceptor
    {
        private readonly IPublisher _publisher;
        private readonly ICurrentUserService _currentUser;

        public AuditPublishingInterceptor(IPublisher publisher, ICurrentUserService currentUser)
        {
            _publisher = publisher;
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
            var entries = context.ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified || e.State == EntityState.Deleted)
                // حماية من الدوران اللانهائي (Infinite Loop Protection)
                .Where(e => !e.Metadata.Name.Contains("Outbox") &&
                            !e.Metadata.Name.Contains("Audit") &&
                            !e.Metadata.Name.Contains("IntegrationEvent"))
                .ToList();

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
                    else if (entry.State == EntityState.Modified)
                    {
                        if (prop.IsModified)
                        {
                            oldVal[propName] = prop.OriginalValue;
                            newVal[propName] = prop.CurrentValue;
                            changedCols.Add(propName);
                        }
                    }
                }

                if (entry.State == EntityState.Modified && changedCols.Count == 0) continue;

                // --- 1. استخراج CommitteeId بذكاء ---
                string? committeeId = null;
                var committeeProp = entry.Entity.GetType().GetProperty("CommitteeId");
                if (committeeProp != null)
                {
                    var val = committeeProp.GetValue(entry.Entity);
                    committeeId = val?.ToString();
                }

                // --- 2. تحديد مستوى الخطورة ---
                string severity = entry.State == EntityState.Deleted ? "Warning" : "Info";

                // --- 3. إنشاء الحدث المحلي (باستخدام بيانات المستخدم الحقيقي) ---
                var localEvent = new AuditLogEvent(
                    EntityName: entry.Entity.GetType().Name,
                    PrimaryKey: entry.Properties.FirstOrDefault(p => p.Metadata.IsPrimaryKey())?.CurrentValue?.ToString() ?? "N/A",
                    ActionType: entry.State.ToString(),

                    //  استخدام الخدمة المحقونة لجلب بيانات المستخدم
                    UserId: _currentUser.UserId.ToString(),
                    UserName: _currentUser.UserName ?? "Anonymous",
                    Email: _currentUser.Email ?? "NoEmail",

                    CommitteeId: committeeId,
                    Justification: null,
                    Severity: severity,

                    OldValues: oldVal.Count > 0 ? JsonSerializer.Serialize(oldVal) : null,
                    NewValues: newVal.Count > 0 ? JsonSerializer.Serialize(newVal) : null,
                    ChangedColumns: changedCols.Count > 0 ? string.Join(",", changedCols) : null
                );

                await _publisher.Publish(localEvent, ct);
            }
        }
    }

}
