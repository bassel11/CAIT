namespace BuildingBlocks.Contracts.Decision.IntegrationEvents
{
    public record DecisionUpdatedIntegrationEvent(
        Guid DecisionId,
        string Title,
        string TextArabic,
        string TextEnglish,
        string Type
    );
}
