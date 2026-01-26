namespace IntegrationService.Application.Interfaces
{
    public record MeetingIntegrationResult(string OutlookEventId, string TeamsJoinUrl);

    public interface IMeetingPlatformService
    {
        Task<MeetingIntegrationResult> CreateOnlineMeetingAsync(
            string subject,
            string content,
            DateTime startUtc,
            DateTime endUtc,
            List<string> attendeeEmails);

        Task CancelMeetingAsync(string outlookEventId, string cancellationReason);
        Task UpdateMeetingTimeAsync(string outlookEventId, DateTime newStartUtc, DateTime newEndUtc);
    }
}
