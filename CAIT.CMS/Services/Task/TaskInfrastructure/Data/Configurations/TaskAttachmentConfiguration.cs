namespace TaskInfrastructure.Data.Configurations
{
    public class TaskAttachmentConfiguration : IEntityTypeConfiguration<TaskAttachment>
    {
        public void Configure(EntityTypeBuilder<TaskAttachment> builder)
        {
            // 1. Primary Key
            builder.HasKey(a => a.Id);
            builder.Property(a => a.Id)
                .HasConversion(id => id.Value, value => TaskAttachmentId.Of(value));

            // 2. Relationship
            builder.HasOne(a => a.TaskItem)
                .WithMany(t => t.TaskAttachments)
                .HasForeignKey(a => a.TaskItemId)
                .OnDelete(DeleteBehavior.Cascade);

            // 3. Properties Conversions
            builder.Property(a => a.UploadedByUserId)
                .HasConversion(id => id.Value, value => UserId.Of(value))
                .IsRequired();

            // 4. File Details Configuration
            builder.Property(a => a.FileName)
                .HasMaxLength(255)
                .IsRequired();

            builder.Property(a => a.BlobPath)
                .HasMaxLength(500) // مسارات التخزين السحابي قد تكون طويلة
                .IsRequired();

            builder.Property(a => a.ContentType)
                .HasMaxLength(100) // e.g., "application/vnd.openxmlformats-officedocument..."
                .IsRequired();

            builder.Property(a => a.SizeInBytes)
                .IsRequired();

            // 5. Versioning
            builder.Property(a => a.Version)
                .IsRequired();

            // فهرس مركب (Composite Index) لمنع تكرار الإصدار لنفس الملف في نفس المهمة (اختياري لزيادة الأمان)
            // يعني: لا يمكن أن يكون هناك ملفين بنفس الاسم ونفس الإصدار لنفس المهمة
            builder.HasIndex(a => new { a.TaskItemId, a.FileName, a.Version })
                .IsUnique();
        }
    }
}
