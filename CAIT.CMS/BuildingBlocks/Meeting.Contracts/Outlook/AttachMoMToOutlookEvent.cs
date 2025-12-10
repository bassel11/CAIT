namespace Meeting.Contracts.Outlook
{
    public record AttachMoMToOutlookEvent(Guid MeetingId, string FileUrl);
}
