namespace TaskInfrastructure.Data.Configurations
{
    public class TaskItemConfiguration : IEntityTypeConfiguration<TaskItem>
    {
        public void Configure(EntityTypeBuilder<TaskItem> builder)
        {
            // 1. Primary Key & Value Object Conversion
            builder.HasKey(t => t.Id);
            builder.Property(t => t.Id)
                .HasConversion(id => id.Value, value => TaskItemId.Of(value));

            // 2. Complex Value Objects (Titles, Descriptions, etc.)
            // نفترض أن لديك Value Objects لهذه الحقول، أو سيتم معاملتها كـ Primitive
            builder.Property(t => t.Title)
                .HasConversion(t => t.Value, v => TaskTitle.Of(v)) // افتراض وجود ValueObject
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(t => t.Description)
                .HasConversion(d => d.Value, v => TaskDescription.Of(v))
                .HasMaxLength(2000);

            builder.Property(t => t.Deadline)
                .HasConversion(d => d != null ? d.Value : (DateTime?)null, v => v.HasValue ? TaskDeadline.Of(v.Value) : null);

            // 3. Enums (Store as String for readability or Int for performance)
            // Enterprise Preference: String prevents issues if Enum order changes
            builder.Property(t => t.Status)
                .HasConversion<string>()
                .HasMaxLength(50);

            builder.Property(t => t.Priority)
                .HasConversion<string>()
                .HasMaxLength(20);

            builder.Property(t => t.Category)
                 .HasConversion<string>()
                 .HasMaxLength(50);

            // 4. Other IDs (References)
            builder.Property(t => t.CommitteeId)
                .HasConversion(id => id.Value, value => CommitteeId.Of(value));

            builder.Property(t => t.MeetingId)
                .HasConversion(id => id != null ? id.Value : (Guid?)null, value => value.HasValue ? MeetingId.Of(value.Value) : null);

            builder.Property(t => t.DecisionId)
               .HasConversion(id => id != null ? id.Value : (Guid?)null, value => value.HasValue ? DecisionId.Of(value.Value) : null);

            builder.Property(t => t.MoMId)
               .HasConversion(id => id != null ? id.Value : (Guid?)null, value => value.HasValue ? MoMId.Of(value.Value) : null);


            // 5. Handling Private Collections (Backing Fields) 🛡️
            // هذا مهم جداً لكي يستطيع EF Core الكتابة في القوائم الخاصة private readonly

            builder.HasMany(t => t.TaskAssignees)
                .WithOne(a => a.TaskItem)
                .HasForeignKey(a => a.TaskItemId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.Metadata.FindNavigation(nameof(TaskItem.TaskAssignees))!
                .SetPropertyAccessMode(PropertyAccessMode.Field); // Access _taskassignees directly

            builder.HasMany(t => t.TaskNotes)
                .WithOne(n => n.TaskItem)
                .HasForeignKey(n => n.TaskItemId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.Metadata.FindNavigation(nameof(TaskItem.TaskNotes))!
                .SetPropertyAccessMode(PropertyAccessMode.Field);

            builder.HasMany(t => t.TaskAttachments)
                .WithOne(a => a.TaskItem)
                .HasForeignKey(a => a.TaskItemId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.Metadata.FindNavigation(nameof(TaskItem.TaskAttachments))!
                .SetPropertyAccessMode(PropertyAccessMode.Field);

            builder.HasMany(t => t.TaskHistories)
                .WithOne()
                .HasForeignKey(h => h.TaskItemId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.Metadata.FindNavigation(nameof(TaskItem.TaskHistories))!
                .SetPropertyAccessMode(PropertyAccessMode.Field);
        }
    }
}
