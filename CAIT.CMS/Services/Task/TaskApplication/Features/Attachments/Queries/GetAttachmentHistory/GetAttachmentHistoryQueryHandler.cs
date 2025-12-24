using BuildingBlocks.Shared.CQRS;
using TaskApplication.Common.Interfaces;
using TaskApplication.Dtos;
using TaskCore.ValueObjects;

namespace TaskApplication.Features.Attachments.Queries.GetAttachmentHistory
{
    public class GetAttachmentHistoryQueryHandler : IQueryHandler<GetAttachmentHistoryQuery, List<TaskAttachmentDto>>
    {
        private readonly ITaskRepository _repository;

        public GetAttachmentHistoryQueryHandler(ITaskRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<TaskAttachmentDto>> Handle(GetAttachmentHistoryQuery request, CancellationToken cancellationToken)
        {
            var task = await _repository.GetByIdAsync(TaskItemId.Of(request.TaskId), cancellationToken);
            if (task == null) throw new KeyNotFoundException("Task not found.");

            var currentAttachment = task.TaskAttachments.FirstOrDefault(a => a.Id == TaskAttachmentId.Of(request.CurrentAttachmentId));
            if (currentAttachment == null) throw new KeyNotFoundException("Attachment not found.");

            // ✅ المنطق الذكي: البحث عن كل المرفقات التي لها نفس الاسم في نفس المهمة
            // هذا يعيد لك الإصدار 1، 2، 3... لنفس الملف
            var versions = task.TaskAttachments
                .Where(a => a.FileName.Equals(currentAttachment.FileName, StringComparison.OrdinalIgnoreCase))
                .OrderByDescending(a => a.Version)
                .Select(a => new TaskAttachmentDto(
                    a.Id.Value,
                    a.FileName,
                    a.BlobPath, // أو رابط التحميل
                    a.ContentType,
                    a.SizeInBytes,
                    a.Version,
                    a.UploadedByUserId.Value,
                    "Unknown", // يحتاج جلب الاسم
                    a.UploadedAt
                ))
                .ToList();

            return versions;
        }
    }
}
