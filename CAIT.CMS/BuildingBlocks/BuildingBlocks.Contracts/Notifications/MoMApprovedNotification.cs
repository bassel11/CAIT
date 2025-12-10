namespace BuildingBlocks.Contracts.Notifications
{
    public record MoMApprovedNotification(
    Guid MoMId,
    Guid MeetingId,
    string To,
    string Subject,
    string Body
);
}
