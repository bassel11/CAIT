using MeetingCore.Enums.AttendanceEnums;
using MeetingCore.Enums.MoMEnums;
using MeetingCore.Events.MoMEvents;
using MeetingCore.ValueObjects.AttendanceVO;
using MeetingCore.ValueObjects.MeetingVO;
using MeetingCore.ValueObjects.MinutesVO;
using MeetingCore.ValueObjects.MoMActionItemDraftVO;
using MeetingCore.ValueObjects.MoMAttachmentVO;
using MeetingCore.ValueObjects.MoMAttendanceVO;
using MeetingCore.ValueObjects.MoMDecisionDraftVO;
using MeetingCore.ValueObjects.MoMDiscussionVO;

namespace MeetingCore.Entities
{
    public class MinutesOfMeeting : Entity<MoMId>
    {
        // ================= البيانات الأساسية =================
        public MeetingId MeetingId { get; private set; } = default!;
        public MoMStatus Status { get; private set; }
        public string FullContentHtml { get; private set; } = default!;
        public int VersionNumber { get; private set; }

        // العلاقات
        private readonly List<MoMDecisionDraft> _decisions = new();
        public IReadOnlyCollection<MoMDecisionDraft> Decisions => _decisions.AsReadOnly();

        private readonly List<MoMActionItemDraft> _actionItems = new();
        public IReadOnlyCollection<MoMActionItemDraft> ActionItems => _actionItems.AsReadOnly();


        // لقطة الحضور (جدول جديد)
        private readonly List<MoMAttendance> _attendanceSnapshot = new();
        public IReadOnlyCollection<MoMAttendance> AttendanceSnapshot => _attendanceSnapshot.AsReadOnly();

        // نقاشات البنود (جدول جديد)
        private readonly List<MoMDiscussion> _discussions = new();
        public IReadOnlyCollection<MoMDiscussion> Discussions => _discussions.AsReadOnly();

        // المرفقات والإصدارات
        private readonly List<MoMAttachment> _attachments = new();
        public IReadOnlyCollection<MoMAttachment> Attachments => _attachments.AsReadOnly();

        private readonly List<MinutesVersion> _versions = new();
        public IReadOnlyCollection<MinutesVersion> Versions => _versions.AsReadOnly();

        // بيانات الاعتماد
        public Guid? ApprovedBy { get; set; }
        public DateTime? ApprovedAt { get; set; }

        // Concurrency
        public byte[] RowVersion { get; set; } = default!;


        public MinutesOfMeeting() { }
        public MinutesOfMeeting(MeetingId meetingId, string content, string createdBy)
        {
            Id = MoMId.Of(Guid.NewGuid());
            MeetingId = meetingId;
            FullContentHtml = content;
            Status = MoMStatus.Draft;
            CreatedBy = createdBy;
            CreatedAt = DateTime.UtcNow;
            VersionNumber = 1;
        }

        // ================= إدارة المسودات =================
        public void AddDecision(string title, string text)
        {
            if (Status == MoMStatus.Approved) throw new DomainException("Cannot edit approved MoM.");
            var order = _decisions.Count + 1;
            _decisions.Add(new MoMDecisionDraft(Id, title, text, order));
        }

        public void AddActionItem(string title, Guid? assigneeId, DateTime? dueDate)
        {
            if (Status == MoMStatus.Approved) throw new DomainException("Cannot edit approved MoM.");
            var order = _actionItems.Count + 1;
            _actionItems.Add(new MoMActionItemDraft(Id, title, assigneeId, dueDate, order));
        }

        public void Approve(Guid userId)
        {
            if (Status != MoMStatus.PendingApproval)
                throw new DomainException("Only pending MoMs can be approved.");

            foreach (var decision in _decisions)
            {
                decision.MarkAsApproved();
            }

            foreach (var actionItem in _actionItems)
            {
                actionItem.MarkAsApproved();
            }

            Status = MoMStatus.Approved;
            ApprovedAt = DateTime.UtcNow;
            ApprovedBy = userId;
            LastTimeModified = DateTime.UtcNow;

            var decisionDtos = _decisions.Select(d => new DecisionIntegrationDto(d.Title, d.Text)).ToList();
            var taskDtos = _actionItems.Select(t => new TaskIntegrationDto(
                t.TaskTitle,
                t.AssigneeId ?? Guid.Empty,
                t.DueDate ?? DateTime.UtcNow
            )).ToList();

            AddDomainEvent(new MoMApprovedEvent(
                Id,
                MeetingId,
                userId,
                DateTime.UtcNow,
                decisionDtos,
                taskDtos
            ));
        }


        // ================= إضافة مرفق للمحضر =================
        public void AddAttachment(
            string fileName,
            string contentType,
            long size,
            string storagePath,
            UserId uploadedBy)
        {
            if (Status == MoMStatus.Approved)
                throw new DomainException("Cannot add attachments to an approved Minutes of Meeting.");

            var attachment = new MoMAttachment(
                this.Id,
                fileName,
                contentType,
                size,
                storagePath,
                uploadedBy
            );

            _attachments.Add(attachment);

            LastModifiedBy = uploadedBy.Value.ToString();
            LastTimeModified = DateTime.UtcNow;
        }


        // ... داخل كلاس MinutesOfMeeting ...

        // ================= إدارة المرفقات (Remove) =================
        public void RemoveAttachment(MoMAttachmentId attachmentId)
        {
            if (Status == MoMStatus.Approved || Status == MoMStatus.Archived)
                throw new DomainException("Cannot remove attachments from an approved or archived MoM.");

            var attachment = _attachments.FirstOrDefault(x => x.Id == attachmentId);
            if (attachment == null)
                throw new DomainException("Attachment not found.");

            _attachments.Remove(attachment);
            LastTimeModified = DateTime.UtcNow;
        }

        // أضف هذه الدوال داخل الكلاس لاستكمال دورة الحياة
        public void UpdateContent(string newContent, UserId modifiedBy)
        {
            if (Status == MoMStatus.Approved || Status == MoMStatus.Archived)
                throw new DomainException("Cannot update content of an approved or archived MoM.");

            // تحديث المحتوى
            FullContentHtml = newContent;
            VersionNumber++;
            LastModifiedBy = modifiedBy.Value.ToString();
            LastTimeModified = DateTime.UtcNow;

            // أرشفة النسخة القديمة قبل التحديث
            var version = new MinutesVersion(
                Id,
                FullContentHtml,
                VersionNumber,
                UserId.Of(Guid.Parse(LastModifiedBy ?? CreatedBy)) // افتراض أنك تخزن الـ ID كنص
            );
            _versions.Add(version);


        }

        public void SubmitForApproval()
        {
            if (Status != MoMStatus.Draft && Status != MoMStatus.Rejected)
                throw new DomainException("Only draft or rejected MoMs can be submitted.");

            Status = MoMStatus.PendingApproval;
            LastTimeModified = DateTime.UtcNow;
        }

        public void Reject(string reason, UserId rejectedBy)
        {
            if (Status != MoMStatus.PendingApproval)
                throw new DomainException("Only pending MoMs can be rejected.");

            Status = MoMStatus.Rejected;
            // يمكن إضافة حقل RejectionReason للكيان إذا لزم الأمر، أو الاكتفاء بالتدقيق
            LastModifiedBy = rejectedBy.Value.ToString();
            LastTimeModified = DateTime.UtcNow;
        }

        public void Archive()
        {
            if (Status != MoMStatus.Approved && Status != MoMStatus.Published)
                throw new DomainException("Only approved or published MoMs can be archived.");

            Status = MoMStatus.Archived;
            LastTimeModified = DateTime.UtcNow;
        }

        public void Publish()
        {
            if (Status != MoMStatus.Approved)
                throw new DomainException("Only approved MoMs can be published.");

            Status = MoMStatus.Published;
            LastTimeModified = DateTime.UtcNow;
        }


        public void UpdateDecision(MoMDecisionDraftId decisionId, string newTitle, string newText)
        {
            // 1. التحقق من حالة المحضر
            if (Status == MoMStatus.Approved || Status == MoMStatus.Archived)
                throw new DomainException("Cannot edit decisions in an approved MoM.");

            // 2. البحث عن القرار داخل القائمة
            var decision = _decisions.FirstOrDefault(x => x.Id == decisionId);
            if (decision == null)
                throw new DomainException("Decision not found.");

            // 3. التعديل (يتم استدعاء دالة داخل الكيان الفرعي)
            decision.Update(newTitle, newText);

            LastTimeModified = DateTime.UtcNow;
        }

        public void RemoveDecision(MoMDecisionDraftId decisionId)
        {
            if (Status == MoMStatus.Approved)
                throw new DomainException("Cannot remove decisions from an approved MoM.");

            var decision = _decisions.FirstOrDefault(x => x.Id == decisionId);
            if (decision == null)
                throw new DomainException("Decision not found.");

            _decisions.Remove(decision);

            // خيار إضافي: إعادة ترتيب العناصر المتبقية (Re-indexing SortOrder)
            // ReorderDecisions(); 

            LastTimeModified = DateTime.UtcNow;
        }

        // ================= إدارة المهام (Update & Remove) =================
        public void UpdateActionItem(MoMActionItemDraftId actionId, string taskTitle, Guid? assigneeId, DateTime? dueDate)
        {
            if (Status == MoMStatus.Approved)
                throw new DomainException("Cannot edit action items in an approved MoM.");

            var actionItem = _actionItems.FirstOrDefault(x => x.Id == actionId);
            if (actionItem == null)
                throw new DomainException("Action item not found.");

            actionItem.Update(taskTitle, assigneeId, dueDate);

            LastTimeModified = DateTime.UtcNow;
        }

        public void RemoveActionItem(MoMActionItemDraftId actionId)
        {
            if (Status == MoMStatus.Approved)
                throw new DomainException("Cannot remove action items from an approved MoM.");

            var actionItem = _actionItems.FirstOrDefault(x => x.Id == actionId);
            if (actionItem == null)
                throw new DomainException("Action item not found.");

            _actionItems.Remove(actionItem);
            LastTimeModified = DateTime.UtcNow;
        }


        #region Snapshot

        public void InitializeSnapshot(
            List<Attendance> meetingAttendances,
            List<AgendaItem> meetingAgendaItems,
            Func<UserId, string> getUserNameResolver)
        {
            if (Status != MoMStatus.Draft) throw new DomainException("Can only initialize draft MoM.");

            // 1. نسخ الحضور (تجميد الحالة)
            // ✅ التعديل هنا: مسح القائمة القديمة (إن وجدت) لضمان عدم التكرار عند إعادة التهيئة
            _attendanceSnapshot.Clear();

            foreach (var att in meetingAttendances)
            {
                string name = getUserNameResolver(att.UserId);

                // ✅ منطق ذكي لتوزيع الملاحظات:
                // إذا كان غائباً، نعتبر الملاحظة "سبب غياب".
                // إذا كان حاضراً، نعتبرها "ملاحظة عامة".
                string? absenceReason = att.AttendanceStatus == AttendanceStatus.Absent ? att.Notes : null;
                string? generalNotes = att.AttendanceStatus != AttendanceStatus.Absent ? att.Notes : null;

                _attendanceSnapshot.Add(new MoMAttendance(
                    this.Id,
                    att.UserId,
                    name,
                    att.Role.ToString(),
                    att.AttendanceStatus,
                    absenceReason, // ✅ تمرير السبب فقط إذا كان غائباً
                    generalNotes   // ✅ تمرير الملاحظات في الحالات الأخرى
                ));
            }

            // 2. إنشاء هياكل للنقاش بناءً على الأجندة
            // ✅ التعديل هنا: التحقق من عدم وجود نقاشات مسبقاً لتجنب التكرار
            if (!_discussions.Any())
            {
                foreach (var item in meetingAgendaItems.OrderBy(x => x.SortOrder))
                {
                    _discussions.Add(new MoMDiscussion(
                        this.Id,
                        item.Id,
                        item.Title.Value,
                        "" // المحتوى فارغ في البداية ليقوم المقرر بتعبئته
                    ));
                }
            }
        }
        // ================= دالة لتحديث نقاش بند معين =================
        public void UpdateTopicDiscussion(MoMDiscussionId topicId, string content)
        {
            // 1. التحقق من الحالات المغلقة (Approved OR Archived)
            if (Status == MoMStatus.Approved || Status == MoMStatus.Archived)
                throw new DomainException("Cannot update discussions in an approved or archived MoM.");

            // 2. البحث باستخدام المعرف الصحيح
            var topic = _discussions.FirstOrDefault(x => x.Id == topicId);

            if (topic == null)
                throw new DomainException("Discussion topic not found.");

            // 3. التعديل
            topic.UpdateContent(content);

            // 4. تحديث وقت التعديل للمحضر
            LastTimeModified = DateTime.UtcNow;
        }

        // =========================================================
        // ✅ إدارة النقاشات (إضافة وحذف)
        // =========================================================

        /// <summary>
        /// إضافة بند نقاش طارئ (غير موجود في الأجندة الأصلية)
        /// مثل: ما يستجد من أعمال (AOB)
        /// </summary>
        public void AddAdHocDiscussion(string title, string content)
        {
            // 1. التحقق من الحالة
            EnsureEditable();

            // 2. التحقق من صحة البيانات
            if (string.IsNullOrWhiteSpace(title))
                throw new DomainException("Discussion title is required.");

            // 3. الإضافة (نرسل null للـ AgendaItemId لأنه طارئ)
            var discussion = new MoMDiscussion(
                this.Id,
                null, // لا يوجد بند أجندة مرتبط
                title,
                content
            );

            _discussions.Add(discussion);
            LastTimeModified = DateTime.UtcNow;
        }

        /// <summary>
        /// حذف بند نقاش
        /// </summary>
        public void RemoveDiscussion(MoMDiscussionId discussionId)
        {
            // 1. التحقق من الحالة
            EnsureEditable();

            var discussion = _discussions.FirstOrDefault(x => x.Id == discussionId);
            if (discussion == null) throw new DomainException("Discussion not found.");

            // ⚠️ قاعدة عمل اختيارية (Business Rule):
            // هل نسمح بحذف النقاشات المرتبطة بالأجندة الأصلية؟
            // الأفضل في الأنظمة الحكومية: لا. لأن المحضر يجب أن يعكس الأجندة.
            // إذا لم يناقش البند، يكتب المقرر "تم تأجيل النقاش" بدلاً من حذف السجل.

            if (discussion.OriginalAgendaItemId != null)
            {
                throw new DomainException("Cannot remove a discussion linked to a core Agenda Item. You can clear its content instead.");
            }

            _discussions.Remove(discussion);
            LastTimeModified = DateTime.UtcNow;
        }

        // دالة مساعدة لتقليل التكرار
        private void EnsureEditable()
        {
            if (Status == MoMStatus.Approved || Status == MoMStatus.Archived)
                throw new DomainException("Cannot modify an approved or archived MoM.");
        }


        public void CorrectAttendance(MoMAttendanceId attendanceRowId, AttendanceStatus newStatus, string? notes)
        {
            // 1. التحقق من أن المحضر قابل للتعديل
            if (Status == MoMStatus.Approved || Status == MoMStatus.Archived)
                throw new DomainException("Cannot correct attendance in an approved or archived MoM.");

            // 2. البحث عن السجل
            var attendanceRecord = _attendanceSnapshot.FirstOrDefault(x => x.Id == attendanceRowId);
            if (attendanceRecord == null)
                throw new DomainException("Attendance record not found in this MoM.");

            // 3. التعديل
            attendanceRecord.CorrectStatus(newStatus, notes);

            // 4. تحديث التدقيق
            LastTimeModified = DateTime.UtcNow;
        }
        #endregion



    }
}



