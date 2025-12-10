namespace Meeting.Contracts.Notifications
{
    public record MoMApprovedNotification(
        Guid MeetingId,
        Guid MoMId,
        Guid ApprovedBy,
        string To,
        string Subject,
        string Body,
        DateTime ApprovedAt
    );
}
