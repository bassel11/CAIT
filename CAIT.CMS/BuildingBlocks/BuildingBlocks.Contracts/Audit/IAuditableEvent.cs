using BuildingBlocks.Contracts.Common;

namespace BuildingBlocks.Contracts.Audit
{
    // أي حدث يحتاج لتدقيق يجب أن يرث هذه الواجهة
    public interface IAuditableEvent : IDomainEvent
    {
        string UserId { get; }
        string EntityName { get; }
        string ActionType { get; }
        string PrimaryKey { get; }
    }

    // الرسالة التي سترسل لخدمة Audit
    public interface IAuditLogCreated
    {
        Guid EventId { get; }
        string UserId { get; }
        string ServiceName { get; }
        string EntityName { get; }
        string ActionType { get; }
        string PrimaryKey { get; }
        DateTime Timestamp { get; }
    }
}
