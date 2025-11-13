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
        public DbSet<Permission> Permissions => Set<Permission>();


        #region Pre for new Privilage Method

        public DbSet<UserRolePermReso> UserRolePermResos => Set<UserRolePermReso>();
        public DbSet<RolePermission> RolePermissions => Set<RolePermission>();
        #endregion

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
            builder.Entity<Permission>(b => b.ToTable("Permissions"));
            //builder.Entity<RolePermission>(b => b.ToTable("RolePermissions"));
            //builder.Entity<Resource>(b => b.ToTable("Resources"));

            #region Pre Tables for new privilage method
            builder.Entity<UserRolePermReso>(b => b.ToTable("UserRolePermResos"));
            builder.Entity<RolePermission>(b => b.ToTable("RolePermissions"));
            #endregion

            // =====================================================
            // 🔹 Roles Configuration: unique constraint على Name
            // =====================================================
            builder.Entity<ApplicationRole>(entity =>
            {
                // الحقل Name فريد الآن
                entity.HasIndex(r => r.Name)
                      .IsUnique(true)
                      .HasDatabaseName("UX_Roles_Name");
            });

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
            //  UserPasswordHistory Configuration
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


            // =====================================================
            // 🔹 Permissions Configuration
            // =====================================================
            builder.Entity<Permission>(entity =>
            {

                entity.HasKey(p => p.Id);

                entity.Property(p => p.Name)
                      .IsRequired()
                      .HasMaxLength(150);

                // تخزين Enums كقيم int في قاعدة البيانات
                entity.Property(p => p.ResourceType)
                      .HasConversion<int>()
                      .IsRequired();

                entity.Property(p => p.Action)
                      .HasConversion<int>()
                      .IsRequired();

                entity.Property(p => p.Description)
                      .HasMaxLength(300);

                entity.HasIndex(p => p.Name)
                      .IsUnique(true)
                      .HasDatabaseName("UX_Permissions_Name");
            });


            // =====================================================
            // 🔹 RolePermissions Configuration (Many-to-Many)
            // =====================================================
            //  builder.Entity<RolePermission>(entity =>
            //  {
            //      // مفتاح مركب بدون ResourceId
            //      //entity.HasKey(rp => new { rp.RoleId, rp.PermissionId, rp.ScopeType });
            //      entity.HasKey(rp => new { rp.RoleId, rp.PermissionId });

            //      entity.Property(rp => rp.ScopeType)
            //            .HasConversion<int>()
            //            .IsRequired();

            //      entity.Property(rp => rp.Allow)
            //            .HasDefaultValue(true);

            //      entity.HasOne(rp => rp.Role)
            //            .WithMany(r => r.RolePermissions)
            //            .HasForeignKey(rp => rp.RoleId)
            //            .OnDelete(DeleteBehavior.Cascade);

            //      entity.HasOne(rp => rp.Permission)
            //            .WithMany(p => p.RolePermissions)
            //            .HasForeignKey(rp => rp.PermissionId)
            //            .OnDelete(DeleteBehavior.Cascade);

            //      entity.HasOne(rp => rp.Resource)
            //            .WithMany(r => r.RolePermissions)
            //            .HasForeignKey(rp => rp.ResourceId)
            //            .OnDelete(DeleteBehavior.Restrict);

            //      entity.HasCheckConstraint("CK_RolePermission_Scope",
            //         @"(ScopeType = 0 AND ResourceId IS NULL)
            //OR (ScopeType IN (1,2) AND ResourceId IS NOT NULL)");
            //  });



            // =====================================================
            //  Resource Configuration
            // =====================================================
            //builder.Entity<Resource>(entity =>
            //{
            //    entity.HasKey(r => r.Id);

            //    entity.Property(r => r.ResourceType)
            //          .HasConversion<int>()
            //          .IsRequired();

            //    entity.Property(r => r.DisplayName)
            //          .HasMaxLength(300);

            //    entity.Property(r => r.CreatedAt)
            //          .HasDefaultValueSql("GETUTCDATE()");

            //    entity.HasIndex(r => new { r.ResourceType, r.ReferenceId })
            //          .IsUnique().HasDatabaseName("UX_Resources_Type_ExternalRef");
            //    entity.HasIndex(r => new { r.ParentResourceType, r.ParentReferenceId })
            //          .HasDatabaseName("IX_Resources_Parent");
            //});

            #region Pre for new Privilage Method

            // UserRolePermReso

            builder.Entity<UserRolePermReso>(entity =>
            {
                entity.HasKey(urpr => urpr.Id);

                entity.HasOne(u => u.User)
                      .WithMany(urpr => urpr.UserRolePermResos)
                      .HasForeignKey(u => u.UserId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(r => r.Role)
                      .WithMany(urpr => urpr.UserRolePermResos)
                      .HasForeignKey(r => r.RoleId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(p => p.Permission)
                      .WithMany(urpr => urpr.UserRolePermResos)
                      .HasForeignKey(p => p.PermissionId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.Property(urpr => urpr.Scope)
                      .HasConversion<int>()
                      .IsRequired();

                entity.Property(urpr => urpr.ResourceType)
                      .HasConversion<int>()
                      .IsRequired();

                entity.Property(urpr => urpr.ParentResourceType)
                      .HasConversion<int>()
                      .IsRequired();

            });

            // RolePermission
            builder.Entity<RolePermission>(entity =>
            {
                // مفتاح مركب
                entity.HasKey(prp => new { prp.RoleId, prp.PermissionId });

                entity.HasOne(p => p.Permission)
                      .WithMany(prp => prp.RolePermissions)
                      .HasForeignKey(p => p.PermissionId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(r => r.Role)
                      .WithMany(prp => prp.RolePermissions)
                      .HasForeignKey(r => r.RoleId)
                      .OnDelete(DeleteBehavior.Cascade);

            });

            #endregion

        }
    }
}
