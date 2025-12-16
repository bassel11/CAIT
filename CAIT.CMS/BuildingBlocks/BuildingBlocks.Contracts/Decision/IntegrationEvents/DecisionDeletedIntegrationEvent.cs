namespace BuildingBlocks.Contracts.Decision.IntegrationEvents
{
    public record DecisionDeletedIntegrationEvent(
        Guid DecisionId,
        string Title
    );
}
