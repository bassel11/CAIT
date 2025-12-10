namespace BuildingBlocks.Contracts.Outlook
{
    public record AttachMoMToOutlookEvent(Guid MeetingId, string PdfUrl);
}
