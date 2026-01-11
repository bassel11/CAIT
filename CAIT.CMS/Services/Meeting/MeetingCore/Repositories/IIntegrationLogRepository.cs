using MeetingCore.Entities;

namespace MeetingCore.Repositories
{
    public interface IIntegrationLogRepository : IAsyncRepository<MeetingIntegrationLog>
    {
        Task<MeetingIntegrationLog> AddLogAsync(MeetingIntegrationLog entity);
    }
}
