namespace BuildingBlocks.Contracts.SecurityEvents
{
    public record UserPermissionsChangedIntegrationEvent(
        Guid UserId
    );
}
