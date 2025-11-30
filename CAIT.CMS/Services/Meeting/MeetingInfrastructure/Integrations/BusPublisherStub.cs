namespace MeetingInfrastructure.Integrations
{
    public class BusPublisherStub
    {
        public Task PublishAsync(string payload)
        {
            // replace with real bus publish
            return Task.CompletedTask;
        }
    }
}
