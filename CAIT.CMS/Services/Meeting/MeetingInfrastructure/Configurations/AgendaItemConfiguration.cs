using MeetingCore.Entities;
using MeetingCore.ValueObjects.AgendaItemVO;
using MeetingCore.ValueObjects.MeetingVO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MeetingInfrastructure.Configurations
{
    public class AgendaItemConfiguration : IEntityTypeConfiguration<AgendaItem>
    {
        public void Configure(EntityTypeBuilder<AgendaItem> builder)
        {
            builder.ToTable("AgendaItems");

            // =========================================================
            // 1. Primary Key
            // =========================================================
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id)
                .HasConversion(
                    id => id.Value,
                    value => AgendaItemId.Of(value));

            // =========================================================
            // 2. Foreign Key (MeetingId)
            // =========================================================
            builder.Property(x => x.MeetingId)
                .HasConversion(
                    id => id.Value,
                    value => MeetingId.Of(value))
                .IsRequired();

            // =========================================================
            // 3. Properties & Value Objects
            // =========================================================

            // Title (String Wrapper)
            builder.Property(x => x.Title)
                .HasConversion(
                    title => title.Value,
                    value => AgendaItemTitle.Of(value))
                .HasMaxLength(500)
                .IsRequired();

            // Description (Primitive String)
            builder.Property(x => x.Description)
                .HasMaxLength(4000);

            // SortOrder (Int Wrapper)
            builder.Property(x => x.SortOrder)
                .HasConversion(
                    order => order.Value,
                    value => SortOrder.Of(value))
                .IsRequired();

            // Duration (Nullable TimeSpan Wrapper)
            // ملاحظة: التعامل مع القيم الفارغة (Nullable) يتطلب تحويلاً شرطياً
            builder.Property(x => x.AllocatedTime)
                .HasConversion(
                    duration => duration != null ? duration.Value : (TimeSpan?)null, // للكتابة
                    value => value.HasValue ? Duration.Of(value.Value) : null // للقراءة
                );

            // PresenterId (Nullable Guid Wrapper)
            builder.Property(x => x.PresenterId)
                .HasConversion(
                    presenter => presenter != null ? presenter.Value : (Guid?)null, // للكتابة
                    value => value.HasValue ? PresenterId.Of(value.Value) : null // للقراءة
                );

            // CreatedAt (Inherited from Entity base class??)
            // إذا كانت الخاصية موجودة في الـ Base Class، نفعل هذا السطر:
            // builder.Property("CreatedAt").IsRequired(); 
            // لكنها غير موجودة في كود Entity<T> الذي أرسلته سابقاً، لذا سأتجاهلها ما لم تضفها للـ Base.

            // =========================================================
            // 4. Indexes
            // =========================================================

            // فهرس للبحث عن بنود اجتماع معين بسرعة
            builder.HasIndex(x => x.MeetingId);

            // فهرس مركب لترتيب البنود داخل الاجتماع (Optimization)
            builder.HasIndex(x => new { x.MeetingId, x.SortOrder }).IsUnique();
        }
    }
}