using BuildingBlocks.Shared.Exceptions;
using BuildingBlocks.Shared.Services;
using CommitteeApplication.Features.Committees.Commands.Models;
using CommitteeCore.Entities;
using CommitteeCore.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CommitteeApplication.Features.Committees.Commands.Handlers
{
    public class ChangeCommitteeStatusCommandHandler : IRequestHandler<ChangeCommitteeStatusCommand, Unit>
    {
        private readonly ICommitteeRepository _committeeRepository;
        private readonly IStatusHistoryRepository _historyRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUser;
        private readonly ILogger<ChangeCommitteeStatusCommandHandler> _logger;

        public ChangeCommitteeStatusCommandHandler(
            ICommitteeRepository committeeRepository,
            IStatusHistoryRepository historyRepository,
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUser,
            ILogger<ChangeCommitteeStatusCommandHandler> logger)
        {
            _committeeRepository = committeeRepository;
            _historyRepository = historyRepository;
            _unitOfWork = unitOfWork;
            _currentUser = currentUser;
            _logger = logger;
        }

        public async Task<Unit> Handle(ChangeCommitteeStatusCommand request, CancellationToken cancellationToken)
        {
            // 1. بدء الترانزكشن (لضمان الذرية Atomicity)
            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            try
            {
                // 2. جلب اللجنة والتحقق منها
                var committee = await _committeeRepository.GetByIdAsync(request.CommitteeId);
                if (committee == null)
                    throw new NotFoundException(nameof(Committee), request.CommitteeId);

                // التحقق: هل الحالة الجديدة مختلفة؟
                if (committee.StatusId == request.NewStatusId)
                    throw new DomainException("New status implies no change. The committee is already in this status.");

                // 3. إنشاء سجل التاريخ (History Record)
                // نستخدم الحالة الحالية من الـ Database لتكون OldStatusId (مصدر موثوق)
                var historyRecord = new CommitteeStatusHistory
                {
                    // Id = Guid.NewGuid(), // يتم إنشاؤه تلقائياً في الـ Entity
                    CommitteeId = committee.Id,
                    OldStatusId = committee.StatusId,
                    NewStatusId = request.NewStatusId,
                    DecisionText = request.DecisionText,
                    DecisionDocumentUrl = request.DecisionDocumentUrl,
                    ChangedByUserId = _currentUser.UserId,
                    ChangedAt = DateTime.UtcNow
                };

                // 4. إضافة التاريخ
                // (سيقوم الـ Repo باستدعاء SaveChanges، وسيعمل الـ Audit Interceptor)
                // (لكن لن يتم الاعتماد في قاعدة البيانات لأن الترانزكشن مفتوح)
                await _historyRepository.AddAsync(historyRecord);

                // 5. تحديث حالة اللجنة
                committee.StatusId = request.NewStatusId;

                // (سيقوم الـ Repo باستدعاء SaveChanges، وسيعمل الـ Audit Interceptor مرة ثانية)
                await _committeeRepository.UpdateAsync(committee);

                // 6. اعتماد كل العمليات (Commit)
                // هنا يتم تثبيت: اللجنة + التاريخ + سجلات الـ Audit + رسائل الـ Outbox
                await _unitOfWork.CommitAsync(cancellationToken);

                _logger.LogInformation("Successfully changed status for Committee {CommitteeId} from {OldStatus} to {NewStatus}",
                    committee.Id, historyRecord.OldStatusId, request.NewStatusId);

                return Unit.Value;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error changing status for Committee {CommitteeId}", request.CommitteeId);

                // 7. تراجع كامل في حال الخطأ
                await _unitOfWork.RollbackAsync(cancellationToken);
                throw; // إعادة رمي الخطأ ليظهر للـ API Response
            }
        }
    }
}
