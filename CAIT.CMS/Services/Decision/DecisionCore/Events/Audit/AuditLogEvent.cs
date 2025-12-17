namespace DecisionCore.Events.Audit
{
    // هذا الحدث داخلي فقط، لن يخرج خارج الخدمة
    public record AuditLogEvent(
        string EntityName,
        string PrimaryKey,
        string ActionType,
        string UserId,
        // 👇 الحقول الجديدة لدعم متطلبات CMS.docx
        string UserName,
        string Email,
        string? CommitteeId,
        string? Justification,
        string Severity,
        // 👆 --------------------------------
        string? OldValues,
        string? NewValues,
        string? ChangedColumns
    ) : INotification;
}
