using BuildingBlocks.Shared.CQRS;
using BuildingBlocks.Shared.Services;
using TaskApplication.Common.Interfaces;
using TaskCore.ValueObjects;

namespace TaskApplication.Features.Attachments.Queries.GetAttachment
{
    public class GetAttachmentContentQueryHandler : IQueryHandler<GetAttachmentContentQuery, FileDownloadResponse>
    {
        private readonly ITaskRepository _repository;
        private readonly IFileStorageService _storageService;
        private readonly ICurrentUserService _currentUser;

        public GetAttachmentContentQueryHandler(
            ITaskRepository repository,
            IFileStorageService storageService,
            ICurrentUserService currentUser)
        {
            _repository = repository;
            _storageService = storageService;
            _currentUser = currentUser;
        }

        public async Task<FileDownloadResponse> Handle(GetAttachmentContentQuery request, CancellationToken cancellationToken)
        {
            var task = await _repository.GetByIdAsync(TaskItemId.Of(request.TaskId), cancellationToken);
            if (task == null) throw new KeyNotFoundException("Task not found.");

            // 1. التحقق من الصلاحية (Security Check)
            // هل المستخدم معين للمهمة؟ (أو أدمن)
            if (!task.IsUserAssigned(UserId.Of(_currentUser.UserId)))
                throw new UnauthorizedAccessException("You do not have permission to view this file.");

            // 2. العثور على المرفق داخل المهمة
            var attachment = task.TaskAttachments.FirstOrDefault(a => a.Id == TaskAttachmentId.Of(request.AttachmentId));
            if (attachment == null) throw new KeyNotFoundException("Attachment not found.");

            // 3. تحميل الملف الفعلي من التخزين
            var fileDto = await _storageService.DownloadAsync(attachment.BlobPath, cancellationToken);

            // نستخدم ContentType المخزن في الداتابيز لأنه أدق
            return new FileDownloadResponse(fileDto.FileStream, attachment.ContentType, attachment.FileName);
        }
    }
}
