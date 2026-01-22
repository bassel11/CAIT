using MeetingCore.Entities;
using MeetingCore.Enums;
using MeetingCore.ValueObjects.AIGeneratedContentVO;
using MeetingCore.ValueObjects.MeetingVO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MeetingInfrastructure.Configurations
{
    public class AIGeneratedContentConfiguration : IEntityTypeConfiguration<AIGeneratedContent>
    {
        public void Configure(EntityTypeBuilder<AIGeneratedContent> builder)
        {
            builder.ToTable("AIGeneratedContents");

            // =========================================================
            // 1. Primary Key
            // =========================================================
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id)
                .HasConversion(
                    id => id.Value,
                    value => AIContentId.Of(value));

            // =========================================================
            // 2. Foreign Keys
            // =========================================================
            builder.Property(x => x.MeetingId)
                .HasConversion(
                    id => id.Value,
                    value => MeetingId.Of(value))
                .IsRequired();

            // =========================================================
            // 3. Enum Conversions (String for Readability)
            // =========================================================
            builder.Property(x => x.ContentType)
                .HasConversion(
                    v => v.ToString(),
                    v => (AIContentType)Enum.Parse(typeof(AIContentType), v))
                .HasMaxLength(50)
                .IsRequired();

            // =========================================================
            // 4. Data Properties
            // =========================================================

            // Prompt: قد يكون طويلاً ولكنه ليس لا نهائياً
            builder.Property(x => x.Prompt)
                .HasMaxLength(4000)
                .IsRequired();

            // GeneratedText: هذا هو المحتوى الأساسي (أجندة/محضر) وقد يكون كبيراً جداً
            builder.Property(x => x.GeneratedText)
                .HasColumnType("nvarchar(max)") // Explicitly set to Max
                .IsRequired();

            // ModelUsed: لتحليل التكاليف والأداء (e.g., "gpt-4-turbo")
            builder.Property(x => x.ModelUsed)
                .HasMaxLength(100)
                .IsRequired();

            // Feedback Loop
            builder.Property(x => x.IsApplied)
                .HasDefaultValue(false);

            // =========================================================
            // 5. Audit Properties
            // =========================================================
            builder.Property(x => x.CreatedBy)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(x => x.CreatedAt)
                .IsRequired();

            // =========================================================
            // 6. Indexes
            // =========================================================

            // للوصول السريع لمحتوى اجتماع معين
            builder.HasIndex(x => x.MeetingId);

            // للتحليلات: مثلاً "كم عدد المحاضر التي تم توليدها بالذكاء الاصطناعي؟"
            builder.HasIndex(x => x.ContentType);

            // للتحليلات: "كم مرة تم استخدام gpt-4 مقابل gpt-3.5؟"
            builder.HasIndex(x => x.ModelUsed);
        }
    }
}