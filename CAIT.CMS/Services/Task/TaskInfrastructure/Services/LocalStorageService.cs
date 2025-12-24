using Microsoft.AspNetCore.Hosting;
using TaskApplication.Common.Interfaces;
using TaskApplication.Dtos;

namespace TaskInfrastructure.Services
{
    public class LocalStorageService : IFileStorageService
    {
        private readonly IWebHostEnvironment _environment;
        private const string UploadsFolder = "uploads";

        public LocalStorageService(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        public async Task<string> UploadAsync(Stream fileStream, string fileName, string contentType, CancellationToken cancellationToken)
        {
            // 1. تحديد المسار الجذري (يفضل wwwroot لسهولة الوصول عبر الروابط لاحقاً)
            var webRootPath = _environment.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            var targetDirectory = Path.Combine(webRootPath, UploadsFolder);

            // 2. إنشاء المجلد إذا لم يكن موجوداً
            if (!Directory.Exists(targetDirectory))
            {
                Directory.CreateDirectory(targetDirectory);
            }

            // 3. تأمين اسم الملف (تجنب التكرار والاختراق)
            var uniqueFileName = $"{Guid.NewGuid()}{Path.GetExtension(fileName)}";
            var filePath = Path.Combine(targetDirectory, uniqueFileName);

            // 4. الحفظ الفعلي
            using (var targetStream = new FileStream(filePath, FileMode.Create))
            {
                await fileStream.CopyToAsync(targetStream, cancellationToken);
            }

            // 5. إرجاع المسار النسبي (URL) الذي يمكن للفرونت إند استخدامه
            // النتيجة ستكون مثل: /uploads/gu-id-gu-id.pdf
            return $"/{UploadsFolder}/{uniqueFileName}";
        }

        public async Task DeleteAsync(string blobPath, CancellationToken cancellationToken)
        {
            // تحويل المسار النسبي (URL) إلى مسار فعلي على القرص
            // blobPath example: /uploads/guid-file.pdf
            var webRootPath = _environment.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

            // إزالة السلاش من البداية وتغيير الفواصل لتناسب نظام التشغيل
            var relativePath = blobPath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar);
            var fullPath = Path.Combine(webRootPath, relativePath);

            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }
            await Task.CompletedTask;
        }

        public async Task<FileDownloadDto> DownloadAsync(string blobPath, CancellationToken cancellationToken)
        {
            // تحويل المسار النسبي (URL) إلى مسار فعلي
            // مثال: /uploads/GUID_file.pdf -> C:\...\wwwroot\uploads\GUID_file.pdf
            var fileName = Path.GetFileName(blobPath);
            var fullPath = Path.Combine(_environment.WebRootPath ?? "wwwroot", "uploads", fileName);

            if (!File.Exists(fullPath))
                throw new FileNotFoundException("File not found on server.");

            var memoryStream = new MemoryStream();
            using (var fileStream = new FileStream(fullPath, FileMode.Open, FileAccess.Read))
            {
                await fileStream.CopyToAsync(memoryStream, cancellationToken);
            }
            memoryStream.Position = 0; // إعادة المؤشر للبداية

            // تخمين نوع الملف (أو يمكن تخزينه في قاعدة البيانات وتمريره هنا، وهو الأفضل)
            var contentType = "application/octet-stream"; // Default

            return new FileDownloadDto(memoryStream, contentType, fileName);
        }

    }
}
