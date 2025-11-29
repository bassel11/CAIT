using MeetingApplication.Interfaces.Integrations;

namespace MeetingInfrastructure.Integrations
{
    public class StorageService : IStorageService
    {
        public Task<byte[]?> GetFileAsync(string storagePath, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task<string> SaveFileAsync(byte[] content, string filename, string contentType, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }
    }
}
