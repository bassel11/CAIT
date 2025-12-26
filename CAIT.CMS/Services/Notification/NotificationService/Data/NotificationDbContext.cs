using MassTransit;
using Microsoft.EntityFrameworkCore;
using NotificationService.Entities;

namespace NotificationService.Data
{
    public class NotificationDbContext : DbContext
    {
        public NotificationDbContext(DbContextOptions<NotificationDbContext> options) : base(options)
        {
        }

        public DbSet<AppNotification> AppNotifications => Set<AppNotification>();
        public DbSet<UserDeviceToken> UserDeviceTokens => Set<UserDeviceToken>();
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // تحسين الأداء: فهرسة UserId لأنه الأكثر استخداماً في البحث
            modelBuilder.Entity<AppNotification>().HasIndex(n => n.UserId);
            modelBuilder.Entity<UserDeviceToken>().HasIndex(t => t.UserId);

            // إعداد جداول Inbox/Outbox الخاصة بـ MassTransit
            modelBuilder.AddInboxStateEntity();
            modelBuilder.AddOutboxMessageEntity();
            modelBuilder.AddOutboxStateEntity();
        }
    }
}
