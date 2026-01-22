using MeetingCore.Entities;

namespace MeetingApplication.Data
{
    public interface IMeetingDbContext
    {
        DbSet<Meeting> Meetings { get; }
        DbSet<AgendaItem> AgendaItems { get; }
        DbSet<Attendance> Attendances { get; }
        DbSet<MinutesOfMeeting> Minutes { get; }
        DbSet<MinutesVersion> MinutesVersions { get; }
        DbSet<MoMAttachment> MoMAttachments { get; }
        DbSet<MoMDecisionDraft> MoMDecisionDrafts { get; }
        DbSet<MoMActionItemDraft> MoMActionItemDrafts { get; }
        DbSet<AIGeneratedContent> AIGeneratedContents { get; }

    }
}
