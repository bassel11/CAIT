namespace BuildingBlocks.Contracts.Decision.IntegrationEvents
{
    public record DecisionCreatedIntegrationEvent(
        Guid DecisionId,
        Guid MeetingId,
        string Title,
        string TextArabic,
        string TextEnglish
    );
}
