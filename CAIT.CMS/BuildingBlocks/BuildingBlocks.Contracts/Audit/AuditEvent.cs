namespace BuildingBlocks.Contracts.Audit
{
    public record AuditEvent(
    Guid EventId,
    string EventType,
    string Payload,
    string ServiceName,
    DateTime OccurredAt
);
}
