using Audit.Application.DTOs;
using Audit.Application.Services;
using Audit.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Audit.Infrastructure.Services
{
    public class AuditQueryNewService : IAuditQueryNewService
    {
        private readonly AuditDbContext _context;

        public AuditQueryNewService(AuditDbContext context)
        {
            _context = context;
        }

        public async Task<List<AuditHistoryDto>> GetHistoryAsync(string entityName, string primaryKey)
        {
            // 1. جلب السجلات المرتبطة بالكيان المحدد (مثلاً قرار رقم 5)
            // الترتيب: الأحدث أولاً
            var logs = await _context.AuditLogs
                .Where(x => x.EntityName == entityName && x.PrimaryKey.Contains(primaryKey))
                .OrderByDescending(x => x.Timestamp)
                .ToListAsync();

            var result = new List<AuditHistoryDto>();

            foreach (var log in logs)
            {
                var dto = new AuditHistoryDto
                {
                    AuditId = log.Id,
                    Action = log.ActionType,
                    User = log.UserName,
                    Email = log.Email ?? "Unknown",
                    Timestamp = log.Timestamp,
                    TimeAgo = GetTimeAgo(log.Timestamp)
                };

                // 2. فك التشفير والمقارنة (Diff Logic)
                var oldValues = string.IsNullOrEmpty(log.OldValues)
                    ? new Dictionary<string, object>()
                    : JsonSerializer.Deserialize<Dictionary<string, object>>(log.OldValues);

                var newValues = string.IsNullOrEmpty(log.NewValues)
                    ? new Dictionary<string, object>()
                    : JsonSerializer.Deserialize<Dictionary<string, object>>(log.NewValues);

                // دمج المفاتيح لمعرفة كل الحقول المتأثرة
                var allKeys = (newValues?.Keys ?? Enumerable.Empty<string>())
                              .Union(oldValues?.Keys ?? Enumerable.Empty<string>());

                foreach (var key in allKeys)
                {
                    var oldVal = oldValues != null && oldValues.ContainsKey(key) ? oldValues[key]?.ToString() : "null";
                    var newVal = newValues != null && newValues.ContainsKey(key) ? newValues[key]?.ToString() : "null";

                    // إضافة التغيير للقائمة
                    dto.Changes.Add(new AuditChangeDto
                    {
                        FieldName = key,
                        OldValue = oldVal,
                        NewValue = newVal
                    });
                }

                result.Add(dto);
            }

            return result;
        }

        private static string GetTimeAgo(DateTime dateTime)
        {
            var span = DateTime.UtcNow - dateTime;
            if (span.TotalMinutes < 1) return "Just now";
            if (span.TotalHours < 1) return $"{span.Minutes}m ago";
            if (span.TotalDays < 1) return $"{span.Hours}h ago";
            return $"{span.Days}d ago";
        }
    }
}
