//namespace BuildingBlocks.Contracts.Audit
//{
//    // Generic Audit envelope used by producers
//    public record AuditEvent(
//        Guid EventId,
//        string EventType,   // e.g., "IAuditLogCreated" or domain event full type
//        string ServiceName, // producer service name
//        DateTime OccurredAt,// UTC
//        string Payload      // JSON object with structured fields (entity/action/user/old/new/primaryKey/etc)
//    );

//    // Optional strongly-typed contract (recommended for producers)
//    public interface IAuditLogCreated
//    {
//        Guid EventId { get; }
//        string UserId { get; }
//        string ServiceName { get; }
//        string EntityName { get; }
//        string ActionType { get; }   // Create/Update/Delete
//        string PrimaryKey { get; }
//        string? OldValues { get; }   // JSON
//        string? NewValues { get; }   // JSON
//        DateTime Timestamp { get; }
//    }
//}
