using BuildingBlocks.Shared.Wrappers;

namespace MeetingApplication.Interfaces.AI
{
    public interface IAiAgendaService
    {
        // الدالة تأخذ "هدف الاجتماع" وتعيد قائمة مقترحة من البنود
        Task<Result<List<string>>> GenerateAgendaSuggestionsAsync(string meetingPurpose, string meetingTitle);
    }
}
