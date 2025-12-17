using Audit.Domain.Entities;
using Audit.Infrastructure.Data;
using BuildingBlocks.Contracts.Audit;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace Audit.Infrastructure.Consumers
{
    public class AuditLogCreatedConsumer : IConsumer<AuditLogCreatedIntegrationEvent>
    {
        private readonly AuditDbContext _dbContext;
        private readonly ILogger<AuditLogCreatedConsumer> _logger;

        // تمت إزالة IPublishEndpoint لأنك لا تحتاج إشعارات حالياً
        public AuditLogCreatedConsumer(AuditDbContext dbContext, ILogger<AuditLogCreatedConsumer> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<AuditLogCreatedIntegrationEvent> context)
        {
            var msg = context.Message;

            // 1. جلب آخر سجل لربط السلسلة (Blockchain Logic)
            // الترتيب حسب الوقت ثم وقت الوصول لضمان الدقة
            var lastLog = await _dbContext.AuditLogs
                .OrderByDescending(x => x.Timestamp)
                .ThenByDescending(x => x.ReceivedAt)
                .FirstOrDefaultAsync();

            // إذا كان الجدول فارغاً نبدأ بسلسلة أصفار (Genesis Hash)
            string previousHash = lastLog?.Hash ?? new string('0', 64);

            // 2. بناء الكيان مع كافة الحقول (بما فيها Email)
            var auditLog = new AuditLog
            {
                Id = Guid.NewGuid(),
                EventId = msg.EventId,
                ServiceName = msg.ServiceName,
                EntityName = msg.EntityName,
                PrimaryKey = msg.PrimaryKey,
                ActionType = msg.ActionType,
                CommitteeId = msg.CommitteeId, // [المصدر: 29]

                // بيانات المستخدم
                UserId = msg.UserId,
                UserName = msg.UserName,
                Email = msg.Email,             // ✅ تمت إضافته

                Justification = msg.Justification, // [المصدر: 3]
                Severity = msg.Severity,

                OldValues = msg.OldValues,
                NewValues = msg.NewValues,
                ChangedColumns = msg.ChangedColumns,

                Timestamp = msg.Timestamp,
                ReceivedAt = DateTime.UtcNow,

                // تخزين البيانات الخام كدليل قطعي
                RawPayload = JsonSerializer.Serialize(msg),

                PreviousHash = previousHash
            };

            // 3. حساب الهاش الآمن (Checksum) [المصدر: 23]
            // يجب أن يتضمن الـ Email لضمان عدم استبدال هوية المستخدم لاحقاً
            auditLog.Hash = CalculateSecureHash(auditLog);

            // 4. الحفظ
            _dbContext.AuditLogs.Add(auditLog);
            await _dbContext.SaveChangesAsync();

            _logger.LogInformation("✅ Audit Log Saved: {Action} on {Entity} by {User}", msg.ActionType, msg.EntityName, msg.Email);
        }

        private static string CalculateSecureHash(AuditLog log)
        {
            // دمج الحقول الحساسة (تمت إضافة Email و RawPayload للمعادلة)
            // الترتيب هنا مقدس! أي تغيير في الترتيب سيغير الهاش
            var rawData = $"{log.Id}|{log.EventId}|{log.ServiceName}|{log.CommitteeId}|{log.EntityName}|{log.ActionType}|{log.Justification}|{log.UserId}|{log.Email}|{log.RawPayload}|{log.Timestamp:O}|{log.PreviousHash}";

            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(rawData));

            // إرجاع الهاش كسلسلة نصية Hex (64 حرفاً)
            return Convert.ToHexString(bytes).ToLowerInvariant();
        }
    }
}