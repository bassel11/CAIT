using Identity.Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Identity.Infrastructure.Data
{
    public class ApplicationDbContext : IdentityDbContext<
        ApplicationUser,
        ApplicationRole,
        Guid,
        IdentityUserClaim<Guid>,
        ApplicationUserRole,
        IdentityUserLogin<Guid>,
        IdentityRoleClaim<Guid>,
        IdentityUserToken<Guid>>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
        public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
        public DbSet<UserPasswordHistory> UserPasswordHistories => Set<UserPasswordHistory>();


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // =====================================================
            // 🔹 Identity Table Naming
            // =====================================================
            builder.Entity<ApplicationUser>(b => b.ToTable("Users"));
            builder.Entity<ApplicationRole>(b => b.ToTable("Roles"));
            builder.Entity<ApplicationUserRole>(b => b.ToTable("UserRoles"));
            builder.Entity<IdentityUserClaim<Guid>>(b => b.ToTable("UserClaims"));
            builder.Entity<IdentityUserLogin<Guid>>(b => b.ToTable("UserLogins"));
            builder.Entity<IdentityRoleClaim<Guid>>(b => b.ToTable("RoleClaims"));
            builder.Entity<IdentityUserToken<Guid>>(b => b.ToTable("UserTokens"));

            // =====================================================
            // 🔹 UserRoles Relationships
            // =====================================================
            builder.Entity<ApplicationUserRole>()
                .HasKey(ur => new { ur.UserId, ur.RoleId });

            builder.Entity<ApplicationUserRole>()
                .HasOne(ur => ur.Role)
                .WithMany(r => r.UserRoles)
                .HasForeignKey(ur => ur.RoleId)
                .OnDelete(DeleteBehavior.Restrict) // 🚫 يمنع حذف الدور إن كان مرتبطًا بمستخدمين
                .IsRequired();

            builder.Entity<ApplicationUserRole>()
                .HasOne(ur => ur.User)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(ur => ur.UserId)
                .OnDelete(DeleteBehavior.Restrict) // 🚫 يمنع حذف المستخدم إذا كان مرتبطًا بدور
                .IsRequired();



            //builder.Entity<ApplicationUserRole>()
            //    .HasOne(ur => ur.User)
            //    .WithMany(u => u.UserRoles)
            //    .HasForeignKey(ur => ur.UserId)
            //    .IsRequired();

            //builder.Entity<ApplicationUserRole>()
            //    .HasOne(ur => ur.Role)
            //    .WithMany(r => r.UserRoles)
            //    .HasForeignKey(ur => ur.RoleId)
            //    .IsRequired();

            // =====================================================
            // 🔹 RefreshToken Relationship
            // =====================================================
            builder.Entity<RefreshToken>()
                .HasOne(rt => rt.User)
                .WithMany(u => u.RefreshTokens)
                .HasForeignKey(rt => rt.UserId);

            // =====================================================
            // 🔹 AuditLog Relationship
            // =====================================================
            builder.Entity<AuditLog>()
                .HasOne(al => al.User)
                .WithMany()
                .HasForeignKey(al => al.UserId);

            // =====================================================
            // 🔹 ApplicationUser Configuration (important part)
            // =====================================================
            builder.Entity<ApplicationUser>(entity =>
            {
                // ---- Field Size & Encoding ----
                //entity.Property(u => u.FirstName)
                //      .HasMaxLength(100)
                //      .IsUnicode(true);

                //entity.Property(u => u.LastName)
                //      .HasMaxLength(100)
                //      .IsUnicode(true);

                //entity.Property(u => u.AdDomain)
                //      .HasMaxLength(100)
                //      .IsUnicode(false);

                //entity.Property(u => u.AdAccount)
                //      .HasMaxLength(255)
                //      .IsUnicode(false);

                //entity.Property(u => u.Email)
                //      .HasMaxLength(255)
                //      .IsUnicode(false);

                // ---- Indexes & Constraints ----

                // فهرس لمستخدمي On-Prem Active Directory
                entity.HasIndex(u => new { u.AdDomain, u.AdAccount })
                      .IsUnique(false)
                      .HasDatabaseName("IX_Users_AD");

                // فهرس لمستخدمي Azure Entra ID
                entity.HasIndex(u => new { u.AzureTenantId, u.AzureObjectId })
                      .IsUnique(false)
                      .HasDatabaseName("IX_Users_Entra");

                // فهرس للبريد الإلكتروني (قد يكون غير فريد)
                entity.HasIndex(u => u.Email)
                      .IsUnique(false)
                      .HasDatabaseName("IX_Users_Email");

                // ---- Default Values ----
                entity.Property(u => u.CreatedAt)
                      .HasDefaultValueSql("GETUTCDATE()");
            });

            // =====================================================
            // 🔹 UserPasswordHistory Configuration ✅
            // =====================================================
            builder.Entity<UserPasswordHistory>(entity =>
            {
                entity.ToTable("UserPasswordHistories");

                entity.HasKey(p => p.Id);

                entity.Property(p => p.PasswordHash)
                      .IsRequired()
                      .HasMaxLength(512);

                entity.Property(p => p.CreatedAt)
                      .HasDefaultValueSql("GETUTCDATE()");

                entity.HasOne(p => p.User)
                      .WithMany()
                      .HasForeignKey(p => p.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
