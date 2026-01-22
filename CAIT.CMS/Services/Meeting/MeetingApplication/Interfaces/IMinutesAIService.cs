namespace MeetingApplication.Interfaces
{
    public interface IMinutesAIService
    {
        // خدمة لتوليد المحضر بناءً على التسجيل الصوتي أو الملاحظات
        Task<string> GenerateContentAsync(string meetingContext, string transcript, CancellationToken ct);
    }
}
