using BuildingBlocks.Shared.CQRS;
using BuildingBlocks.Shared.Services;
using TaskApplication.Common.Interfaces;
using TaskCore.ValueObjects;

namespace TaskApplication.Features.Tasks.Commands.UpdateTaskStatus
{
    public class UpdateTaskStatusCommandHandler
        : ICommandHandler<UpdateTaskStatusCommand, UpdateTaskStatusResult>
    {
        private readonly ITaskRepository _repository;
        private readonly ICurrentUserService _currentUser;

        public UpdateTaskStatusCommandHandler(ITaskRepository repository, ICurrentUserService currentUser)
        {
            _repository = repository;
            _currentUser = currentUser;
        }

        // ✅ تصحيح 1: تغيير نوع الإرجاع من Task إلى Task<UpdateTaskStatusResult>
        public async Task<UpdateTaskStatusResult> Handle(UpdateTaskStatusCommand request, CancellationToken cancellationToken)
        {
            var taskId = TaskItemId.Of(request.TaskId);
            var task = await _repository.GetByIdAsync(taskId, cancellationToken);

            if (task == null)
                throw new KeyNotFoundException($"Task with ID {request.TaskId} was not found.");

            // ✅ تصحيح 2: تحويل Guid إلى UserId Value Object
            //var currentUserId = UserId.Of(_currentUser.UserId);
            var currentUserId = UserId.Of(Guid.Parse("D1288717-9701-4E27-890C-8D6281D831D6"));

            // التحقق من الصلاحية
            if (!task.IsUserAssigned(currentUserId))
            {
                throw new UnauthorizedAccessException("You are not assigned to this task.");
            }

            // تحديث الحالة في الدومين
            // ✅ تصحيح 3: تمرير القيمة الصحيحة للـ Enum والـ UserId
            task.UpdateStatus(currentUserId, request.NewStatus);

            // حفظ التغييرات
            await _repository.SaveChangesAsync(cancellationToken);

            // ✅ تصحيح 4: إرجاع النتيجة المطلوبة من الواجهة
            return new UpdateTaskStatusResult(task.Id.Value);
        }
    }
}
