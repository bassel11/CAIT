namespace Meeting.Contracts.Integration
{
    public record MoMApprovedIntegrationEvent(Guid MoMId, Guid MeetingId, DateTime ApprovedAt);
}
