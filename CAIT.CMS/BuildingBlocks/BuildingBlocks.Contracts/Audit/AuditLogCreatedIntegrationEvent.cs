namespace BuildingBlocks.Contracts.Audit
{
    public record AuditLogCreatedIntegrationEvent(
       Guid EventId,
       string ServiceName,      // e.g., DecisionService, IdentityService
       string EntityName,       // e.g., Committee, Meeting, Decision
       string PrimaryKey,       // ID of the record
       string ActionType,       // Create, Update, Delete, Suspend, LoginFailed [المصدر: 11-18]
       string? CommitteeId,     // هام جداً للفلترة حسب اللجنة [المصدر: 29]
       string UserId,           // [المصدر: 5]
       string UserName,         // لتسهيل العرض في التقارير
       string Email,            // البريد الالكتروني للمستخدم
       string? Justification,   // [المصدر: 3] سبب الإجراء (مهم للعمليات الحساسة)
       string Severity,         // Info, Warning, Critical (لدعم التنبيهات [المصدر: 36])
       string? OldValues,       // JSON
       string? NewValues,       // JSON
       string? ChangedColumns,
       DateTime Timestamp       // [المصدر: 5]
   );
}
