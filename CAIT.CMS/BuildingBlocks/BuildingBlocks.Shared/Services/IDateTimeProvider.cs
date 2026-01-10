namespace BuildingBlocks.Shared.Services
{
    public interface IDateTimeProvider
    {
        DateTime UtcNow { get; }
        DateTime Now { get; }

    }
}
