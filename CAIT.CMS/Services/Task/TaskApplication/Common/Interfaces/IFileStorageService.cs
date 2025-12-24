using TaskApplication.Dtos;

namespace TaskApplication.Common.Interfaces
{
    public interface IFileStorageService
    {
        // يرفع الملف ويعيد المسار (Blob URI or Path)
        Task<string> UploadAsync(Stream fileStream, string fileName, string contentType, CancellationToken cancellationToken);

        // إضافة دالة الحذف للتراجع
        Task DeleteAsync(string blobPath, CancellationToken cancellationToken);

        Task<FileDownloadDto> DownloadAsync(string blobPath, CancellationToken cancellationToken);
    }
}
