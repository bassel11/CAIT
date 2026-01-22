using MeetingApplication.Integrations;

namespace MeetingInfrastructure.Services
{
    public class AgendaGeneratorService : IAgendaGeneratorService
    {
        public async Task<List<AgendaItemDto>> GenerateAsync(string meetingTitle, string purpose, CancellationToken cancellationToken)
        {
            // محاكاة تأخير الشبكة
            await Task.Delay(500, cancellationToken);

            // محاكاة الذكاء الاصطناعي
            return new List<AgendaItemDto>
            {
                new("Introduction", $"Opening remarks for {meetingTitle}", 5),
                new("Context Overview", "Reviewing the background information.", 10),
                new("Deep Dive: " + purpose, "Main discussion regarding the core purpose.", 30),
                new("Q&A Session", "Open floor for questions.", 10),
                new("Closing & Action Items", "Wrap up and assign tasks.", 5)
            };
        }
    }
}
