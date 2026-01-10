using BuildingBlocks.Shared.CQRS;
using BuildingBlocks.Shared.Exceptions;
using BuildingBlocks.Shared.Services;
using TaskApplication.Common.Interfaces;
using TaskCore.ValueObjects;

namespace TaskApplication.Features.Tasks.Commands.UploadAttachment
{
    public class UploadAttachmentCommandHandler
        : ICommandHandler<UploadAttachmentCommand, UploadAttachmentResult>
    {
        private readonly ITaskRepository _repository;
        private readonly IFileStorageService _storageService;
        private readonly ICurrentUserService _currentUser;

        public UploadAttachmentCommandHandler(
            ITaskRepository repository,
            IFileStorageService storageService,
            ICurrentUserService currentUser)
        {
            _repository = repository;
            _storageService = storageService;
            _currentUser = currentUser;
        }

        public async Task<UploadAttachmentResult> Handle(UploadAttachmentCommand request, CancellationToken cancellationToken)
        {
            var task = await _repository.GetByIdAsync(TaskItemId.Of(request.TaskId), cancellationToken);
            if (task == null)
                throw new NotFoundException("Task", request.TaskId);

            // 1. Upload to Azure Blob Storage (via Interface)
            // Path: tasks/{taskId}/{fileName}
            var blobPath = await _storageService.UploadAsync(
                request.FileStream,
                request.FileName,
                request.ContentType,
                cancellationToken
            );

            try
            {
                // 2. Add to Domain (قد يفشل هنا بسبب قواعد العمل Validation)
                task.AddAttachment(
                    UserId.Of(_currentUser.UserId),
                    request.FileName,
                    blobPath,
                    request.ContentType,
                    request.Size
                );

                // 3. Save to DB (قد يفشل هنا بسبب مشاكل الاتصال)
                await _repository.SaveChangesAsync(cancellationToken);

                var lastAttachment = task.TaskAttachments.Last();
                return new UploadAttachmentResult(lastAttachment.Id.Value);
            }
            catch (Exception)
            {
                // ⚠️ Compensating Action (عملية تعويضية)
                // حدث خطأ في الدومين أو الداتابيز، يجب حذف الملف الذي تم رفعه للتو
                // لكي لا نترك ملفات "يتيمة" (Orphaned Files) في السيرفر
                try
                {
                    await _storageService.DeleteAsync(blobPath, cancellationToken);
                }
                catch
                {
                    // Log error here: "Failed to rollback file upload"
                    // لا نرمي خطأ الحذف، بل نركز على الخطأ الأصلي، لكن يجب تسجيل هذا الفشل في السيرفر
                }

                // إعادة رمي الخطأ ليظهر للفرونت إند
                throw;
            }
        }
    }
}
