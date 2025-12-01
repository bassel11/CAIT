namespace MeetingApplication.Integrations
{
    public interface ITeamsService
    {
        Task<string> FetchTranscriptUrlAsync(Guid meetingId);
        Task<byte[]> DownloadTranscriptAsync(string url);
    }
}
