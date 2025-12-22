namespace TaskInfrastructure.Data.Configurations
{
    public class TaskNoteConfiguration : IEntityTypeConfiguration<TaskNote>
    {
        public void Configure(EntityTypeBuilder<TaskNote> builder)
        {
            // 1. Primary Key
            builder.HasKey(n => n.Id);
            builder.Property(n => n.Id)
                .HasConversion(id => id.Value, value => TaskNoteId.Of(value));

            // 2. Foreign Key & Relationship
            // العلاقة تم تعريفها في TaskItemConfiguration، ولكن نؤكد الربط هنا
            builder.HasOne(n => n.TaskItem)
                .WithMany(t => t.TaskNotes)
                .HasForeignKey(n => n.TaskItemId)
                .OnDelete(DeleteBehavior.Cascade); // حذف الملاحظات عند حذف المهمة

            // 3. Properties & Conversions
            builder.Property(n => n.UserId)
                .HasConversion(id => id.Value, value => UserId.Of(value))
                .IsRequired();

            builder.Property(n => n.Content)
                .HasMaxLength(2000) // تحديد حد أقصى للنص
                .IsRequired();

            builder.Property(n => n.IsDeleted)
                .HasDefaultValue(false);

            // تكوين فلاتر الاستعلام العالمية (Global Query Filter) لعدم جلب المحذوفات افتراضياً
            // هذا اختياري، ولكن مفيد جداً في حالات الـ Soft Delete
            builder.HasQueryFilter(n => !n.IsDeleted);
        }
    }
}
