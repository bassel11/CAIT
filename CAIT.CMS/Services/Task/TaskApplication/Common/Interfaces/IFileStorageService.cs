namespace TaskApplication.Common.Interfaces
{
    public interface IFileStorageService
    {
        // يرفع الملف ويعيد المسار (Blob URI or Path)
        Task<string> UploadAsync(Stream fileStream, string fileName, string contentType, CancellationToken cancellationToken);
    }
}
