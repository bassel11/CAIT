using TaskCore.Enums;
using TaskCore.Events.TaskAssigneeEvents;
using TaskCore.Events.TaskAttachmentEvents;
using TaskCore.Events.TaskItemEvents;
using TaskCore.Events.TaskNoteEvents;

namespace TaskCore.Entities
{
    public class TaskItem : Aggregate<TaskItemId>
    {
        // Collections
        private readonly List<TaskAssignee> _taskAssignees = new();
        public IReadOnlyList<TaskAssignee> TaskAssignees => _taskAssignees.AsReadOnly();

        private readonly List<TaskNote> _taskNotes = new();
        public IReadOnlyList<TaskNote> TaskNotes => _taskNotes.AsReadOnly();

        private readonly List<TaskAttachment> _taskAttachments = new();
        public IReadOnlyList<TaskAttachment> TaskAttachments => _taskAttachments.AsReadOnly();

        private readonly List<TaskHistory> _taskHistories = new();
        public IReadOnlyList<TaskHistory> TaskHistories => _taskHistories.AsReadOnly();

        // Fields
        public TaskTitle Title { get; private set; } = default!;
        public TaskDescription Description { get; private set; } = default!;
        public TaskDeadline? Deadline { get; private set; } = default!;
        public TaskPriority Priority { get; private set; } = TaskPriority.Low;
        public TaskCategory Category { get; private set; }
        public Enums.TaskStatus Status { get; private set; } = Enums.TaskStatus.NotStarted;

        // References
        public CommitteeId CommitteeId { get; private set; } = default!;
        public MeetingId? MeetingId { get; private set; } = default!;
        public DecisionId? DecisionId { get; private set; } = default!;
        public MoMId? MoMId { get; private set; } = default!;

        // Constructors
        private TaskItem() { } // For EF Core

        public static TaskItem Create(
           TaskItemId id,
           TaskTitle title,
           TaskDescription description,
           TaskDeadline? deadline,
           TaskPriority priority,
           TaskCategory category,
           CommitteeId committeeId,
           MeetingId? meetingId = null,
           DecisionId? decisionId = null,
           MoMId? momId = null)
        {
            var taskItem = new TaskItem
            {
                Id = id,
                Title = title,
                Description = description,
                Deadline = deadline,
                Priority = priority,
                Category = category,
                CommitteeId = committeeId,
                MeetingId = meetingId,
                DecisionId = decisionId,
                MoMId = momId,
                Status = Enums.TaskStatus.NotStarted
            };

            taskItem.AddDomainEvent(new TaskItemCreatedEvent(taskItem));

            return taskItem;
        }

        #region Factory Methods for Integration

        // ✅ مصنع مخصص للمهام القادمة من المحضر
        public static TaskItem CreateFromMoM(
            TaskItemId id,
            TaskTitle title,
            TaskDescription description, // يمكن أن يكون فارغاً أو نفس العنوان مؤقتاً
            TaskDeadline? deadline,
            CommitteeId committeeId,
            MeetingId meetingId,
            MoMId momId)
        {
            var task = new TaskItem
            {
                Id = id,
                Title = title,
                // عادةً المهام في المحضر تكون سطر واحد، لذا نضع العنوان في الوصف أيضاً أو نتركه فارغاً
                Description = description,
                Deadline = deadline,

                // القيم الافتراضية لمهام المحضر
                Priority = TaskPriority.Medium, // افتراضي
                Category = TaskCategory.Operational, // افتراضي
                Status = Enums.TaskStatus.NotStarted,

                // الروابط المرجعية
                CommitteeId = committeeId,
                MeetingId = meetingId,
                MoMId = momId
            };

            task.AddDomainEvent(new TaskItemCreatedEvent(task));
            return task;
        }

        #endregion


        public void UpdateDetails(
                    UserId modifierId,
                    TaskTitle newTitle,
                    TaskDescription newDescription,
                    TaskPriority newPriority,
                    TaskCategory newCategory,
                    TaskDeadline? newDeadline)
        {
            // التحقق من أن المهمة ليست مغلقة أو مؤرشفة (حسب قواعد العمل)
            if (Status == Enums.TaskStatus.Completed || Status == Enums.TaskStatus.Cancelled)
            {
                throw new DomainException("Cannot update details of a completed or cancelled task.");
            }

            // نحتفظ بالقيم القديمة للمقارنة (اختياري للتدقيق الدقيق)
            var oldTitle = Title;
            // ... يمكن تكرار ذلك لبقية الحقول

            // تحديث القيم
            Title = newTitle;
            Description = newDescription;
            Priority = newPriority;
            Category = newCategory;
            Deadline = newDeadline;

            // تسجيل التاريخ (Audit)
            LogHistory(
                modifierId,
                TaskHistoryAction.Updated,
                "Task details updated (Title, Desc, Priority, etc.)"
            );

            // إطلاق حدث (اختياري، لو أردت إشعار أحد بتغيير التفاصيل)
            AddDomainEvent(new TaskDetailsUpdatedEvent(Id, modifierId));
        }
        public void AssignUser(UserId userId, string email, string name) // 👈 نستخدم Value Object
        {
            // لا داعي للتحقق من userId == empty هنا، لأن UserId.Of قام بذلك مسبقاً

            if (string.IsNullOrWhiteSpace(name))
                throw new DomainException("Name cannot be empty");

            if (string.IsNullOrWhiteSpace(email))
                throw new DomainException("Email cannot be empty");

            // التحقق من التكرار أصبح مباشراً
            if (!_taskAssignees.Any(a => a.UserId == userId))
            {
                // نمرر الـ Value Object مباشرة
                var taskassignee = new TaskAssignee(Id, userId, email, name);
                _taskAssignees.Add(taskassignee);

                AddDomainEvent(new TaskAssignUserEvent(Id, taskassignee));
            }
        }

        public void UnassignUser(UserId userId)
        {
            var assignee = _taskAssignees.FirstOrDefault(a => a.UserId == userId);

            // Idempotency: إذا لم يكن موجوداً أصلاً، لا نفعل شيئاً (أو يمكن رمي خطأ حسب البزنس)
            if (assignee == null) return;

            _taskAssignees.Remove(assignee);

            // إطلاق حدث الإزالة (مهم لإرسال إشعار للمستخدم بأنه لم يعد مسؤولاً)
            AddDomainEvent(new TaskUnassignUserEvent(Id, assignee));
        }

        public bool IsUserAssigned(UserId userId)
        {
            return _taskAssignees.Any(a => a.UserId == userId);
        }

        public IEnumerable<UserId> GetAssignedUserIds()
        {
            return _taskAssignees.Select(a => a.UserId);
        }

        // 5. GetAssigneesDictionary: للبحث السريع (مفيد عند التعامل مع مهام جماعية كبيرة)
        public IReadOnlyDictionary<UserId, TaskAssignee> GetAssigneesDictionary()
        {
            // بما أن UserId هو Record، يمكن استخدامه كـ Key في القاموس بأمان
            return _taskAssignees.ToDictionary(a => a.UserId);
        }


        public void AddNote(UserId userId, string content)
        {
            // 1. التحقق من الصلاحية (اختياري في الدومين، لكن يفضل التأكد أن المستخدم له علاقة بالمهمة)
            // حسب المتطلبات: "Assigned members can add notes"
            // إذا أردت التشدد، يمكنك تفعيل السطر التالي:
            if (!IsUserAssigned(userId)) throw new DomainException("Only assigned members can add notes.");

            // 2. إنشاء الملاحظة
            var note = new TaskNote(Id, userId, content);
            _taskNotes.Add(note);

            // 3. إطلاق حدث (مهم جداً لإشعار المقرر أو الرئيس حسب المتطلبات)
            AddDomainEvent(new TaskNoteAddedEvent(Id, userId, content));
        }

        public void EditNote(UserId userId, TaskNoteId noteId, string newContent)
        {
            var note = _taskNotes.FirstOrDefault(n => n.Id == noteId);

            if (note == null) throw new DomainException("Note not found.");

            if (note.UserId != userId)
                throw new DomainException("You can only edit your own notes.");

            if (note.IsDeleted)
                throw new DomainException("Cannot edit a deleted note.");

            // ✅ نحتفظ بالمحتوى القديم قبل التعديل
            var oldContent = note.Content;

            // نقوم بالتحديث
            note.UpdateContent(newContent);

            // ✅ نمرر القديم والجديد للحدث لغايات التدقيق (Audit)
            AddDomainEvent(new TaskNoteUpdatedEvent(Id, noteId, userId, oldContent, newContent));
        }
        // 2. حذف ملاحظة (Soft Delete)
        public void RemoveNote(UserId userId, TaskNoteId noteId)
        {
            var note = _taskNotes.FirstOrDefault(n => n.Id == noteId);

            if (note == null) throw new DomainException("Note not found.");

            // التحقق من الصلاحية: هل المستخدم هو صاحب الملاحظة؟
            if (note.UserId != userId)
                throw new DomainException("You can only delete your own notes.");

            // لا يمكن حذف ملاحظة محذوفة مسبقاً
            if (note.IsDeleted) return;

            // ✅ نقوم بنسخ المحتوى قبل الحذف لإرساله مع الحدث
            var contentSnapshot = note.Content;

            note.MarkAsDeleted();

            // ✅ نمرر المحتوى المحذوف إلى الحدث
            AddDomainEvent(new TaskNoteRemovedEvent(Id, noteId, userId, contentSnapshot));
        }

        // ✅ New Attachment Method
        public void AddAttachment(UserId userId, string fileName, string blobPath, string contentType, long sizeInBytes)
        {
            // 1. Check permissions (Optional based on strictness)
            if (!IsUserAssigned(userId))
                throw new DomainException("Only assigned members can upload documents.");

            // 2. Versioning Logic: Find max version for the same filename
            var currentVersion = _taskAttachments
                .Where(a => a.FileName.Equals(fileName, StringComparison.OrdinalIgnoreCase))
                .Max(a => (int?)a.Version) ?? 0;

            var newVersion = currentVersion + 1;

            // 3. Create Attachment Entity
            var attachment = new TaskAttachment(
                Id,
                userId,
                fileName,
                blobPath,
                contentType,
                sizeInBytes,
                newVersion
            );

            _taskAttachments.Add(attachment);

            // 4. Raise Event for Audit
            AddDomainEvent(new TaskAttachmentUploadedEvent(
                Id,
                attachment.Id,
                userId,
                fileName,
                newVersion,
                sizeInBytes
            ));
        }

        // ✅ 1. دالة خاصة لتسجيل التاريخ (لعدم تكرار الكود)
        private void LogHistory(UserId userId, TaskHistoryAction action, string details, string? oldValue = null, string? newValue = null)
        {
            var history = new TaskHistory(Id, userId, action, details, oldValue, newValue);
            _taskHistories.Add(history);

            // ملاحظة: لا نطلق DomainEvent من هنا، لأن الحدث الأصلي (مثل TaskStatusChanged) هو المهم.
            // التاريخ هنا هو مجرد "سجل داخلي" للعرض في الـ UI.
        }

        // ✅ 2. مثال: تعديل الحالة مع تسجيل التاريخ
        public void UpdateStatus(UserId userId, Enums.TaskStatus newStatus)
        {
            if (Status == newStatus) return;

            // التحقق: لا يمكن تغيير الحالة لنفس الحالة الحالية
            if (Status == newStatus) return;

            // التحقق: هل المستخدم معين لهذه المهمة؟ (اختياري حسب البيزنس)
            // if (!IsUserAssigned(userId) && !IsAdmin) throw ...

            ApplyStatusChange(userId, newStatus);
        }

        // ✅ 2. دالة التحقق من التأخير (تستدعى من Background Job)
        public void CheckOverdue(UserId systemUserId)
        {
            // شروط عدم التحويل إلى Overdue:
            // 1. المهمة مكتملة بالفعل (لا يمكن أن تتأخر بعد الإنجاز)
            if (Status == Enums.TaskStatus.Completed) return;

            // 2. المهمة هي أصلاً Overdue (Idempotency)
            if (Status == Enums.TaskStatus.Overdue) return;

            // 3. لا يوجد موعد نهائي أصلاً
            if (Deadline == null) return;

            // المنطق: هل الوقت الحالي تجاوز الموعد النهائي؟
            if (DateTime.UtcNow > Deadline.Value)
            {
                // نقوم بتغيير الحالة، ونمرر معرف "النظام" كـ الفاعل
                ApplyStatusChange(systemUserId, Enums.TaskStatus.Overdue);

                // ملاحظة: هنا سينطلق حدث TaskStatusUpdatedEvent بحالة Overdue
                // خدمة الإشعارات ستلتقط هذا الحدث وترسل "Escalation Email" للرئيس
            }
        }

        // 🔒 دالة مساعدة خاصة لتطبيق التغيير (لمنع تكرار الكود)
        private void ApplyStatusChange(UserId actorId, Enums.TaskStatus newStatus)
        {
            var oldStatus = Status;
            Status = newStatus;

            // 1. تسجيل التاريخ (كما بنيناه سابقاً)
            LogHistory(
                actorId,
                TaskHistoryAction.StatusChanged,
                $"Status changed from {oldStatus} to {newStatus}",
                oldStatus.ToString(),
                newStatus.ToString()
            );

            // 2. إطلاق حدث التغيير
            AddDomainEvent(new TaskStatusUpdatedEvent(Id, actorId, oldStatus, newStatus));
        }


    }
}
