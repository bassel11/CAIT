namespace MeetingApplication.Integrations
{
    public interface IStorageService
    {
        Task<string> SaveFileAsync(byte[] content, string filename, string contentType, CancellationToken ct = default);
        Task<byte[]?> GetFileAsync(string storagePath, CancellationToken ct = default);
    }
}
