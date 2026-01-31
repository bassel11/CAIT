using MeetingCore.Enums.AttendanceEnums;
using MeetingCore.Enums.MeetingEnums;
using MeetingCore.Events.AttendanceEvents;
using MeetingCore.Events.MeetingEvents;
using MeetingCore.ValueObjects.AgendaItemVO;
using MeetingCore.ValueObjects.AIGeneratedContentVO;
using MeetingCore.ValueObjects.AttendanceVO;
using MeetingCore.ValueObjects.MeetingVO;

namespace MeetingCore.Entities
{
    public class Meeting : Aggregate<MeetingId>
    {
        // ================= البيانات الأساسية =================
        public CommitteeId CommitteeId { get; private set; } = default!;
        public MeetingTitle Title { get; private set; } = default!;
        public string? Description { get; private set; }


        // التوقيت (دائماً UTC) والمكان
        public DateTime StartDate { get; private set; }
        public DateTime EndDate { get; private set; }
        public TimeZoneId TimeZone { get; private set; } = TimeZoneId.Utc;
        public MeetingLocation Location { get; private set; } = default!;

        // الحالة والتحكم
        public MeetingStatus Status { get; private set; } = MeetingStatus.Draft;
        public string? CancellationReason { get; private set; } = default!;

        // التكامل (Microsoft Graph)
        public string? TeamsLink { get; private set; }
        public string? OutlookEventId { get; private set; }

        // التكرار
        public RecurrencePattern Recurrence { get; private set; } = default!;

        // Concurrency Control
        public byte[] RowVersion { get; set; } = default!;

        // =========================================================
        // الجديد: سياسة النصاب (Snapshot من اللجنة)
        // =========================================================
        public MeetingQuorumPolicy QuorumPolicy { get; private set; } = default!;
        public bool IsCurrentQuorumMet { get; private set; } = false;

        // ================= العلاقات (Collections) =================
        private readonly List<AgendaItem> _agendaItems = new();
        public IReadOnlyCollection<AgendaItem> AgendaItems => _agendaItems.AsReadOnly();

        private readonly List<Attendance> _attendances = new();
        public IReadOnlyCollection<Attendance> Attendances => _attendances.AsReadOnly();

        private readonly List<AIGeneratedContent> _aiContents = new();
        public IReadOnlyCollection<AIGeneratedContent> AIContents => _aiContents.AsReadOnly();

        // علاقة 1-to-1 مع المحضر
        public MinutesOfMeeting? Minutes { get; private set; }


        // ================= البناء (Construction) =================
        private Meeting() { } // EF Core

        #region Constructor and Create

        public Meeting(
            MeetingId id,
            CommitteeId committeeId,
            MeetingTitle title,
            string? description,
            DateTime startDate,
            DateTime endDate,
            TimeZoneId timeZone,
            MeetingLocation location,
            RecurrencePattern recurrence,
            string? createdBy)
        {
            Id = id ?? throw new DomainException("MeetingId is required.");
            CommitteeId = committeeId ?? throw new DomainException("CommitteeId is required.");
            Title = title ?? throw new DomainException("Title is required.");
            StartDate = startDate;
            EndDate = endDate;
            TimeZone = timeZone ?? TimeZoneId.Utc;
            Location = location ?? throw new DomainException("Location is required.");
            Description = description;
            Recurrence = recurrence ?? RecurrencePattern.None;

            Status = MeetingStatus.Draft;
            CreatedBy = createdBy;
            CreatedAt = DateTime.UtcNow;
        }

        // ================= Factory Method =================
        public static Meeting Create(
            MeetingId id,
            CommitteeId committeeId,
            MeetingTitle title,
            string? description,
            DateTime startDate,
            DateTime endDate,
            TimeZoneId timeZone,
            MeetingLocation location,
            RecurrencePattern recurrence,
            MeetingQuorumPolicy quorumPolicy,
            string? createdBy)
        {
            if (endDate <= startDate)
                throw new DomainException("Meeting end date must be after start date.");

            if (location.Type == LocationType.Physical && string.IsNullOrWhiteSpace(location.RoomName) && string.IsNullOrWhiteSpace(location.Address))
                throw new DomainException("Physical meetings must have a location.");

            var meeting = new Meeting
            {
                Id = id,
                CommitteeId = committeeId,
                Title = title,
                Description = description,
                StartDate = startDate,
                EndDate = endDate,
                TimeZone = timeZone,
                Location = location,
                Recurrence = recurrence,
                Status = MeetingStatus.Draft,
                QuorumPolicy = quorumPolicy ?? throw new DomainException("Quorum Policy is required."),
                CreatedBy = createdBy,
                CreatedAt = DateTime.UtcNow
            };


            meeting.AddDomainEvent(new MeetingCreatedEvent(
                meeting.Id.Value,
                meeting.CommitteeId.Value,
                meeting.Title.Value,
                meeting.StartDate,
                meeting.EndDate,
                meeting.TimeZone.Value,
                createdBy, // تحويل الـ Audit String إلى Guid
                meeting.CreatedAt
    ));
            return meeting;
        }

        #endregion

        #region Quorum Logic
        public bool IsQuorumMet()
        {
            // 1. حساب الأعضاء الذين يحق لهم التصويت
            var votingMembersCount = _attendances.Count(a => a.VotingRight == VotingRight.Voting);

            // إذا لم يوجد مصوتون، النصاب متحقق افتراضياً (اجتماع مفتوح/تشاوري)
            if (votingMembersCount == 0) return true;

            // 2. حساب الحضور الفعلي (بمن فيهم النواب)
            var presentCount = _attendances.Count(a => a.CountsForQuorum());

            // 3. تطبيق السياسة المخزنة
            switch (QuorumPolicy.Type)
            {
                case QuorumType.Percentage:
                    // المعادلة: (الحضور / الكلي) * 100 >= النسبة
                    if (!QuorumPolicy.ThresholdPercent.HasValue) return false;
                    decimal currentPercent = ((decimal)presentCount / votingMembersCount) * 100;
                    return currentPercent >= QuorumPolicy.ThresholdPercent.Value;

                case QuorumType.PercentagePlusOne:
                    // المعادلة: 50% + 1 (عادة تعني النصف + 1)
                    // مثال: 10 أعضاء -> النصف 5 -> المطلوب 6
                    // مثال: 11 عضو -> النصف 5.5 -> المطلوب 6
                    int half = votingMembersCount / 2;
                    return presentCount >= (half + 1);

                case QuorumType.AbsoluteNumber:
                    // المعادلة: عدد ثابت
                    if (!QuorumPolicy.AbsoluteCount.HasValue) return false;
                    return presentCount >= QuorumPolicy.AbsoluteCount.Value;

                default:
                    return false;
            }
        }
        private bool CalculateQuorumStatus()
        {
            // عدد الأعضاء الكلي الذين يحق لهم التصويت (المقام)
            var votingMembersCount = _attendances.Count(a => a.VotingRight == VotingRight.Voting);

            // اجتماعات بدون مصوتين تعتبر قانونية (تشاورية)
            if (votingMembersCount == 0) return true;

            // عدد الحضور الفعلي للمصوتين (البسط)
            var presentCount = _attendances.Count(a => a.CountsForQuorum());

            return QuorumPolicy.Type switch
            {
                QuorumType.Percentage =>
                    QuorumPolicy.ThresholdPercent.HasValue &&
                    ((decimal)presentCount / votingMembersCount) * 100 >= QuorumPolicy.ThresholdPercent.Value,

                QuorumType.PercentagePlusOne =>
                    presentCount >= (votingMembersCount / 2) + 1,

                QuorumType.AbsoluteNumber =>
                    QuorumPolicy.AbsoluteCount.HasValue &&
                    presentCount >= QuorumPolicy.AbsoluteCount.Value,

                _ => false
            };
        }
        private void UpdateQuorumState()
        {
            var newState = CalculateQuorumStatus();

            // نحدث فقط إذا تغيرت الحالة لتقليل عمليات الكتابة والأحداث
            if (IsCurrentQuorumMet != newState)
            {
                IsCurrentQuorumMet = newState;

                // 🔥 إطلاق حدث خاص بتغير الحالة (مفيد للـ Frontend Real-time notifications)
                AddDomainEvent(new MeetingQuorumStatusChangedEvent(
                    Id.Value,
                    IsCurrentQuorumMet,
                    DateTime.UtcNow
                ));
            }
        }
        public void RefreshQuorumPolicy(MeetingQuorumPolicy newPolicy, string modifiedBy)
        {
            // 1. التحقق من الحالة (Invariants)
            // لا يجوز تغيير قواعد اللعبة أثناء اللعب (InProgress) أو بعد انتهائها (Completed)
            if (Status == MeetingStatus.InProgress || Status == MeetingStatus.Completed || Status == MeetingStatus.Cancelled)
            {
                throw new DomainException($"Cannot refresh quorum rules when meeting status is '{Status}'. Rules are locked once the meeting starts.");
            }

            // 2. التحقق من أن السياسة الجديدة مختلفة فعلاً (لتحسين الأداء والتدقيق)
            if (QuorumPolicy.Equals(newPolicy))
            {
                return; // لا داعي لعمل شيء إذا كانت القواعد متطابقة
            }

            // 3. تطبيق التغيير
            var oldPolicyDescription = QuorumPolicy.GetDescription();
            QuorumPolicy = newPolicy;

            // 4. التدقيق
            LastModifiedBy = modifiedBy;
            LastTimeModified = DateTime.UtcNow;


            UpdateQuorumState();

            // 5. إطلاق حدث (مهم جداً هنا لتوثيق تغيير قانوني)
            AddDomainEvent(new MeetingQuorumPolicyUpdatedEvent(
                Id.Value,
                oldPolicyDescription,
                newPolicy.GetDescription(),
                modifiedBy,
                DateTime.UtcNow));
        }


        #endregion

        #region Domain Behaviour

        public void Schedule()
        {
            if (Status != MeetingStatus.Draft)
                throw new DomainException("Only draft meetings can be scheduled.");

            if (!_agendaItems.Any())
                throw new DomainException("Cannot schedule a meeting without an Agenda.");

            Status = MeetingStatus.Scheduled;
            LastTimeModified = DateTime.UtcNow;
            //LastModifiedBy = userId;

            AddDomainEvent(new MeetingScheduledEvent(
                Id,
                CommitteeId,
                Title.Value,
                StartDate,
                EndDate,
                _attendances.Select(a => a.UserId.Value).ToList()));
        }
        public void Complete(string userId)
        {
            // 1. التحقق من القوانين (Invariants)
            if (Status == MeetingStatus.Completed)
                throw new DomainException("Meeting is already completed.");

            if (Status == MeetingStatus.Cancelled)
                throw new DomainException("Cannot complete a cancelled meeting.");

            // يمكن إضافة شرط: لا يمكن إنهاء اجتماع مستقبلي (يجب أن يكون تاريخه قد حان)
            // if (DateTime.UtcNow < StartDate) throw ...

            // 2. تغيير الحالة
            Status = MeetingStatus.Completed;

            // 3. تحديث التدقيق
            LastModifiedBy = userId;
            LastTimeModified = DateTime.UtcNow;

            // 4. إطلاق حدث (مهم جداً هنا لإنشاء مسودة المحضر تلقائياً مثلاً)
            AddDomainEvent(new MeetingCompletedEvent(Id.Value, DateTime.UtcNow));
        }
        public void Reschedule(DateTime newStart, DateTime newEnd, string userId)
        {
            if (Status == MeetingStatus.Cancelled || Status == MeetingStatus.Completed)
                throw new DomainException("Cannot reschedule a finished meeting.");

            StartDate = newStart;
            EndDate = newEnd;
            Status = MeetingStatus.Rescheduled;
            LastModifiedBy = userId;
            LastTimeModified = DateTime.UtcNow;

            AddDomainEvent(new MeetingRescheduledEvent(Id, newStart, newEnd, OutlookEventId));
        }
        public void Cancel(string reason, string? cancelledBy = null)
        {
            if (Status == MeetingStatus.Completed)
                throw new DomainException("Cannot cancel a completed meeting.");

            if (Status == MeetingStatus.Cancelled)
                return;

            Status = MeetingStatus.Cancelled;
            CancellationReason = reason;
            LastModifiedBy = cancelledBy;
            LastTimeModified = DateTime.UtcNow;

            AddDomainEvent(new MeetingCancelledEvent(
                Id,
                reason,
                OutlookEventId,
                _attendances.Select(a => a.UserId.Value).ToList()
            ));
        }

        public void UpdateDetails(
            MeetingTitle title,
            string? description,
            MeetingLocation location,
            string modifiedBy)
        {
            // 1. التحقق من القوانين (Invariants)
            // لا يمكن تعديل تفاصيل اجتماع ملغي أو مكتمل
            if (Status == MeetingStatus.Cancelled || Status == MeetingStatus.Completed)
                throw new DomainException("Cannot update details of a finished meeting.");

            // 2. تطبيق التغييرات
            Title = title;
            Description = description;
            Location = location;

            // 3. تحديث التدقيق
            LastModifiedBy = modifiedBy;
            LastTimeModified = DateTime.UtcNow;

            // 4. إطلاق حدث (اختياري، مفيد لتحديث الـ Read Models)
            // AddDomainEvent(new MeetingDetailsUpdatedEvent(Id, Title.Value, ...));
        }
        public void UpdateIntegrationInfo(string outlookEventId, string teamsLink)
        {
            OutlookEventId = outlookEventId;
            TeamsLink = teamsLink;
        }

        #endregion

        #region Attendance Behaviour
        public void AddAttendee(
            UserId userId,
            AttendanceRole role,
            VotingRight votingRight)
        {
            if (Status == MeetingStatus.Completed || Status == MeetingStatus.Cancelled)
                throw new DomainException("Cannot add attendee to a closed meeting.");

            if (_attendances.Any(a => a.UserId == userId))
                return;

            _attendances.Add(
              new Attendance(
                    Id,
                    userId,
                    role,
                    votingRight));

            UpdateQuorumState();
        }
        public void RemoveAttendee(UserId userId)
        {
            if (Status == MeetingStatus.Completed)
                throw new DomainException("Cannot remove attendee from a completed meeting history.");

            var attendance = _attendances.FirstOrDefault(a => a.UserId == userId);
            if (attendance == null) throw new DomainException("Attendee not found.");

            // لا يمكن حذف حضور قام بتسجيل الدخول بالفعل (Business Rule)
            if (attendance.AttendanceStatus != AttendanceStatus.None)
                throw new DomainException("Cannot remove an attendee who has already checked in.");

            _attendances.Remove(attendance);
            LastTimeModified = DateTime.UtcNow;

            UpdateQuorumState();

            AddDomainEvent(new MeetingAttendeeRemovedEvent(Id.Value, userId.Value));
        }

        public void ConfirmRSVP(UserId userId, RSVPStatus status)
        {
            if (Status == MeetingStatus.Completed || Status == MeetingStatus.Cancelled)
            {
                throw new DomainException("Cannot change RSVP for a closed or cancelled meeting.");
            }

            var attendance = _attendances.FirstOrDefault(a => a.UserId == userId);
            if (attendance == null) throw new DomainException("Attendee not found in this meeting.");

            attendance.ConfirmRSVP(status);
            LastTimeModified = DateTime.UtcNow;

            AddDomainEvent(new MeetingAttendeeRSVPedEvent(
                Id.Value,
                userId.Value,
                status,
                DateTime.UtcNow
            ));
        }
        public void CheckInAttendee(UserId userId, bool isRemote, bool isProxy = false, string? proxyName = null)
        {
            bool isCheckInAllowed = Status == MeetingStatus.Scheduled ||
                                    Status == MeetingStatus.Rescheduled ||
                                    Status == MeetingStatus.InProgress;

            // ثم نقول: إذا لم يكن مسموحاً، ارمِ خطأ
            if (!isCheckInAllowed)
            {
                throw new DomainException($"Check-in is not allowed. Current status: {Status}");
            }

            var attendance = _attendances.FirstOrDefault(a => a.UserId == userId);

            // هنا نسمح بالـ CheckIn حتى لو لم يكن مدعواً إذا كانت السياسة تسمح (Walk-in logic)
            // ولكن بناءً على كودك، سنفترض أنه يجب أن يكون في القائمة.
            if (attendance == null) throw new DomainException("User is not listed as an attendee.");

            attendance.CheckIn(isRemote, isProxy, proxyName);
            LastTimeModified = DateTime.UtcNow;

            UpdateQuorumState();
            // نطلق حدث يحتوي على حالة النصاب الجديدة لتحديث الواجهة فوراً
            AddDomainEvent(new MeetingAttendeeCheckedInEvent(
                Id.Value,
                userId.Value,
                attendance.AttendanceStatus,
                isRemote,
                DateTime.UtcNow,
                IsQuorumMet() // ✅ نرسل حالة النصاب الحالية
            ));
        }
        public void BulkCheckIn(List<(UserId UserId, AttendanceStatus Status, bool IsProxy, string? ProxyName)> entries)
        {
            var now = DateTime.UtcNow;
            var changesList = new List<BulkCheckInItem>();

            foreach (var entry in entries)
            {
                var attendance = _attendances.FirstOrDefault(a => a.UserId == entry.UserId);

                // يمكن هنا تطبيق منطق Walk-in إذا لم يوجد
                if (attendance != null)
                {
                    // نستخدم الدالة الذكية لتوحيد المنطق
                    // ملاحظة: SetStatus السابقة كانت بسيطة، الآن نستخدم CheckIn إذا كانت الحالة حضور
                    if (entry.Status == AttendanceStatus.Present || entry.Status == AttendanceStatus.Remote)
                    {
                        attendance.CheckIn(
                            entry.Status == AttendanceStatus.Remote,
                            entry.IsProxy,
                            entry.ProxyName
                        );
                    }
                    else
                    {
                        attendance.MarkAbsent(); // أو SetStatus للغياب
                    }

                    changesList.Add(new BulkCheckInItem(entry.UserId.Value, entry.Status));
                }
            }

            if (changesList.Any())
            {
                LastTimeModified = DateTime.UtcNow;

                UpdateQuorumState();
                // ✅ نرسل حالة النصاب الجديدة مع الحدث
                AddDomainEvent(new MeetingAttendeesBulkCheckedInEvent(
                    Id.Value,
                    changesList,
                    now,
                    IsQuorumMet()
                ));
            }
        }

        #endregion

        #region Agenda Item Behaviour

        public void AddAgendaItem(
           AgendaItemTitle title,
           Duration? allocatedTime,
           SortOrder sortOrder,
           PresenterId? presenterId,
           string? description = null)
        {
            if (_agendaItems.Any(a => a.SortOrder == sortOrder))
                throw new DomainException("Duplicate agenda item order.");

            _agendaItems.Add(new AgendaItem(
                Id,
                title,
                allocatedTime,
                sortOrder,
                presenterId,
                description));

            LastTimeModified = DateTime.UtcNow;
        }

        public void UpdateAgendaItem(
            AgendaItemId itemId,
            AgendaItemTitle title,
            string? description,
            SortOrder sortOrder,
            Duration? allocatedTime,
            PresenterId? presenterId)
        {
            var item = _agendaItems.FirstOrDefault(x => x.Id == itemId);
            if (item == null) throw new DomainException("Agenda item not found in this meeting.");

            // تحديث البند
            item.Update(title, description, sortOrder, allocatedTime, presenterId);

            LastTimeModified = DateTime.UtcNow;
        }

        public void RemoveAgendaItem(AgendaItemId itemId)
        {
            var item = _agendaItems.FirstOrDefault(x => x.Id == itemId);
            if (item == null) throw new DomainException("Agenda item not found.");

            _agendaItems.Remove(item);
            LastTimeModified = DateTime.UtcNow;
        }

        #endregion

        #region AIGenerate

        public void LogAIGeneration(
            AIContentType type,
            string prompt,
            string result,
            string modelName,
            UserId userId)
        {
            // يمكن وضع قيود هنا، مثلاً: لا يمكن توليد أجندة لاجتماع منتهي

            var aiContent = new AIGeneratedContent(
                Id,
                type,
                prompt,
                result,
                modelName,
                userId
            );

            _aiContents.Add(aiContent);
        }

        // دالة لتطبيق النص المقترح (مثلاً نسخ الأجندة المقترحة إلى الأجندة الحقيقية)
        public void ApplyAIContent(AIContentId contentId)
        {
            var content = _aiContents.FirstOrDefault(x => x.Id == contentId);
            if (content == null) throw new DomainException("AI Content not found.");

            // نضع علامة أنه تم الاستفادة منه
            content.MarkAsApplied();

            // هنا يمكن تنفيذ المنطق، مثلاً:
            // if (content.ContentType == AIContentType.AgendaDraft) { ... نسخ النصوص ... }
        }
        #endregion

        #region Helpers & Validators Functions
        public void HandleSchedulingFailure(string reason)
        {
            // التحقق: لا يمكن فشل جدولة اجتماع هو أصلاً منتهي أو ملغي
            if (Status == MeetingStatus.Completed || Status == MeetingStatus.Cancelled)
                return; // تجاهل آمن

            // الرجوع إلى Draft أو حالة خاصة (مثلاً PendingRetry)
            // الأفضل هنا: إعادته لـ Draft ليقوم المقرر بتعديل الوقت
            Status = MeetingStatus.Draft;

            // نخزن سبب الفشل في وصف الإلغاء مؤقتاً أو نضيف حقل جديد للملاحظات
            // هنا سنستخدم حقل موجود أو نعتبره ملاحظة تدقيق
            // CancellationReason = $"Scheduling Failed: {reason}"; // خيار

            LastTimeModified = DateTime.UtcNow;

            // نطلق حدثاً داخلياً (Domain Event) قد يفيد في إرسال إشعار للمقرر
            // AddDomainEvent(new MeetingSchedulingFailedDomainEvent(...));
        }
        public void EnsureDecisionsAreAllowed()
        {
            if (!IsCurrentQuorumMet)
            {
                throw new DomainException("Action failed: Quorum is not met. Official decisions cannot be recorded.");
            }
        }

        #endregion
    }
}
