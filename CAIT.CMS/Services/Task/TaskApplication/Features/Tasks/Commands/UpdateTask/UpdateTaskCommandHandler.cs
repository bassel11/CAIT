using BuildingBlocks.Shared.CQRS;
using BuildingBlocks.Shared.Exceptions;
using BuildingBlocks.Shared.Services;
using TaskApplication.Common.Interfaces;
using TaskCore.ValueObjects;

namespace TaskApplication.Features.Tasks.Commands.UpdateTask
{
    public class UpdateTaskCommandHandler
        : ICommandHandler<UpdateTaskCommand, UpdateTaskResult>
    {
        private readonly ITaskRepository _repository;
        private readonly ICurrentUserService _currentUser;

        public UpdateTaskCommandHandler(ITaskRepository repository, ICurrentUserService currentUser)
        {
            _repository = repository;
            _currentUser = currentUser;
        }

        public async Task<UpdateTaskResult> Handle(UpdateTaskCommand request, CancellationToken cancellationToken)
        {
            var task = await _repository.GetByIdAsync(TaskItemId.Of(request.TaskId), cancellationToken);
            if (task == null)
                throw new NotFoundException("Task", request.TaskId);

            // 1. التحقق من الصلاحية (Security)
            // من يحق له تعديل التفاصيل؟
            // الخيار 1: فقط منشئ المهمة (Creator) أو الأدمن.
            // الخيار 2: أي عضو في اللجنة.
            // الخيار 3: الشخص المسؤول (Assignee).

            // سأفترض هنا أفضل ممارسة: "المسؤول أو المنشئ"
            // (بما أننا لم نخزن CreatorId في TaskItem بشكل صريح في الكود السابق، سنعتمد على المسؤول حالياً)
            if (!task.IsUserAssigned(UserId.Of(_currentUser.UserId)))
            {
                // هنا يمكنك إضافة شرط: || _currentUser.IsAdmin
                throw new DomainException("You are not authorized to edit this task details.");
            }

            // 2. التحديث عبر الدومين
            // لاحظ استخدام Factory Methods لإنشاء الـ Value Objects لضمان الـ Validation
            task.UpdateDetails(
                UserId.Of(_currentUser.UserId),
                TaskTitle.Of(request.Title),
                TaskDescription.Of(request.Description),
                request.Priority,
                request.Category,
                request.Deadline.HasValue ? TaskDeadline.Of(request.Deadline.Value) : null
            );

            // 3. الحفظ
            await _repository.SaveChangesAsync(cancellationToken);

            return new UpdateTaskResult(task.Id.Value);
        }
    }
}
