using MeetingCore.ValueObjects.MinutesVO;
using MeetingCore.ValueObjects.MoMDecisionDraftVO;

namespace MeetingCore.Entities
{
    public class MoMDecisionDraft : Entity<MoMDecisionDraftId>
    {
        public MoMId MoMId { get; private set; } = default!;
        public string Title { get; private set; } = default!;
        public string Text { get; private set; } = default!;
        public int SortOrder { get; private set; }
        public DraftStatus Status { get; private set; } = DraftStatus.Draft;

        private MoMDecisionDraft() { }

        internal MoMDecisionDraft(MoMId momId, string title, string text, int order)
        {
            Id = MoMDecisionDraftId.Of(Guid.NewGuid());
            MoMId = momId;
            Title = title;
            Text = text;
            SortOrder = order;
        }

        public void Update(string title, string text)
        {
            if (Status == DraftStatus.Approved)
                throw new DomainException("Approved decision draft is read-only.");

            Title = title;
            Text = text;
        }

        internal void MarkAsApproved()
            => Status = DraftStatus.Approved;
    }
}
