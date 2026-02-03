using MeetingCore.ValueObjects.AgendaItemVO;
using MeetingCore.ValueObjects.MinutesVO;
using MeetingCore.ValueObjects.MoMDiscussionVO;

namespace MeetingCore.Entities
{
    public class MoMDiscussion : Entity<MoMDiscussionId>
    {
        public MoMId MoMId { get; private set; } = default!;

        // المرجع للبند الأصلي (يمكن أن يكون Null لو كان نقاشاً عاماً)
        public AgendaItemId? OriginalAgendaItemId { get; private set; }

        public string TopicTitle { get; private set; } = default!; // عنوان البند
        public string DiscussionContent { get; private set; } = default!; // ما دار من نقاش (HTML)

        private MoMDiscussion() { }

        internal MoMDiscussion(
            MoMId momId,
            AgendaItemId? agendaItemId,
            string title,
            string content)
        {
            Id = MoMDiscussionId.Of(Guid.NewGuid());
            MoMId = momId;
            OriginalAgendaItemId = agendaItemId;
            TopicTitle = title;
            DiscussionContent = content;
        }

        public void UpdateContent(string newContent)
        {
            DiscussionContent = newContent;
        }
    }
}
