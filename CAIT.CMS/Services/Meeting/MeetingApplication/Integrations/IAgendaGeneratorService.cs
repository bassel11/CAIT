namespace MeetingApplication.Integrations
{
    public interface IAgendaGeneratorService
    {
        Task<List<AgendaItemDto>> GenerateAsync(string meetingTitle, string purpose, CancellationToken cancellationToken);
    }

    // DTO بسيط داخلي لنقل البيانات من خدمة الـ AI
    public record AgendaItemDto(string Title, string Description, int DurationMinutes);
}
