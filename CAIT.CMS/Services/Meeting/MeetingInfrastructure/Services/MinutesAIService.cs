using MeetingApplication.Interfaces;

namespace MeetingInfrastructure.Services
{
    public class MinutesAIService : IMinutesAIService
    {
        // هذا مجرد تنفيذ مؤقت (Dummy Implementation) لكي يعمل النظام
        // لاحقاً يمكنك ربطه بـ OpenAI أو Azure AI
        public async Task<string> GenerateContentAsync(string meetingContext, string transcript, CancellationToken ct)
        {
            // محاكاة تأخير
            await Task.Delay(100, ct);

            return $@"
            <h3>Generated MoM for: {meetingContext}</h3>
            <p><strong>Summary:</strong> Based on the transcript provided...</p>
            <ul>
                <li>Point 1 extracted from AI.</li>
                <li>Point 2 extracted from AI.</li>
            </ul>
            <p><em>(This is a dummy AI response)</em></p>
            ";
        }
    }
}
