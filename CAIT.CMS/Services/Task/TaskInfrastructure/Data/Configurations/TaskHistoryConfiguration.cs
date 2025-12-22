namespace TaskInfrastructure.Data.Configurations
{
    public class TaskHistoryConfiguration : IEntityTypeConfiguration<TaskHistory>
    {
        public void Configure(EntityTypeBuilder<TaskHistory> builder)
        {
            // 1. Primary Key
            builder.HasKey(h => h.Id);
            builder.Property(h => h.Id)
                .HasConversion(id => id.Value, value => TaskHistoryId.Of(value));

            // 2. Relationship
            builder.HasOne<TaskItem>() // علاقة أحادية الاتجاه (TaskItem يملك القائمة، لكن History لا يحتاج Navigation Property للعودة)
                .WithMany(t => t.TaskHistories)
                .HasForeignKey(h => h.TaskItemId)
                .OnDelete(DeleteBehavior.Cascade);

            // 3. Properties Conversions
            builder.Property(h => h.UserId)
                .HasConversion(id => id.Value, value => UserId.Of(value))
                .IsRequired();

            // تخزين الـ Enum كنص (String) ليكون مقروءاً في قاعدة البيانات
            builder.Property(h => h.Action)
                .HasConversion<string>()
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(h => h.Details)
                .HasMaxLength(500)
                .IsRequired();

            // القيم القديمة والجديدة (قابلة لتكون null)
            builder.Property(h => h.OldValue)
                .HasMaxLength(200)
                .IsRequired(false);

            builder.Property(h => h.NewValue)
                .HasMaxLength(200)
                .IsRequired(false);

            // 4. Performance Index
            // فهرس لتسريع استرجاع تاريخ مهمة معينة مرتباً زمنياً
            builder.HasIndex(h => h.TaskItemId);
            builder.HasIndex(h => h.Timestamp);
        }
    }
}
