using MeetingApplication.Integrations;

namespace MeetingInfrastructure.Integrations
{
    public class StorageService : IStorageService
    {
        private readonly string _basePath;

        public StorageService()
        {
            _basePath = Path.Combine(Directory.GetCurrentDirectory(), "storage");
            if (!Directory.Exists(_basePath))
                Directory.CreateDirectory(_basePath);
        }

        public async Task<string> SaveFileAsync(byte[] content, string filename, string contentType, CancellationToken ct = default)
        {
            var fullPath = Path.Combine(_basePath, filename);

            await File.WriteAllBytesAsync(fullPath, content, ct);

            // بإمكانك إرجاع المسار أو URL محلي
            return fullPath;
        }

        public async Task<byte[]?> GetFileAsync(string storagePath, CancellationToken ct = default)
        {
            if (!File.Exists(storagePath))
                return null;

            return await File.ReadAllBytesAsync(storagePath, ct);
        }
    }
}