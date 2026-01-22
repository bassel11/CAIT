using MeetingCore.ValueObjects.MinutesVO;
using MeetingCore.ValueObjects.MoMActionItemDraftVO;

namespace MeetingCore.Entities
{
    public class MoMActionItemDraft : Entity<MoMActionItemDraftId>
    {
        public MoMId MoMId { get; private set; } = default!;
        public string TaskTitle { get; private set; } = default!;
        public Guid? AssigneeId { get; private set; } // الموظف المسؤول
        public DateTime? DueDate { get; private set; }
        public int SortOrder { get; private set; }

        public DraftStatus Status { get; private set; } = DraftStatus.Draft;

        private MoMActionItemDraft() { } // EF Core

        internal MoMActionItemDraft(MoMId momId, string title, Guid? assigneeId, DateTime? dueDate, int order)
        {
            Id = MoMActionItemDraftId.Of(Guid.NewGuid());
            MoMId = momId;
            TaskTitle = title;
            AssigneeId = assigneeId;
            DueDate = dueDate;
            SortOrder = order;
        }

        public void Update(string title, Guid? assigneeId, DateTime? dueDate)
        {
            if (Status == DraftStatus.Approved)
                throw new DomainException("Approved action item draft is read-only.");

            TaskTitle = title;
            AssigneeId = assigneeId;
            DueDate = dueDate;
        }

        internal void MarkAsApproved()
        {
            Status = DraftStatus.Approved;
        }
    }

}
