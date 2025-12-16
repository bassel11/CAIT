using DecisionCore.Enums;

namespace DecisionApplication.Dtos
{
    public record UpdateDecisionDto(
        string Title,
        string ArabicText,
        string EnglishText,
        DecisionType Type,
        Guid? AgendaItemId
    );
}
