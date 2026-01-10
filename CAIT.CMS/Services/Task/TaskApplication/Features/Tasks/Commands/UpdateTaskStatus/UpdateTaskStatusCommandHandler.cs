using BuildingBlocks.Shared.CQRS;
using BuildingBlocks.Shared.Exceptions;
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
                throw new NotFoundException("Task", request.TaskId);
            // ✅ تصحيح 2: تحويل Guid إلى UserId Value Object
            var currentUserId = UserId.Of(_currentUser.UserId);

            // التحقق من الصلاحية
            if (!task.IsUserAssigned(currentUserId))
            {
                throw new DomainException("You are not authorized to update this task's status because you are not an assignee.");
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
