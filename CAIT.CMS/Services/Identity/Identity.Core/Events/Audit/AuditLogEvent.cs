using MediatR;

namespace Identity.Core.Events.Audit
{
    public record AuditLogEvent(
    string EntityName,
    string PrimaryKey,
    string ActionType,
    string UserId,
    string UserName,
    string Email,
    string? CommitteeId,
    string? Justification,
    string Severity,
    string? OldValues,
    string? NewValues,
    string? ChangedColumns
) : INotification;

}
