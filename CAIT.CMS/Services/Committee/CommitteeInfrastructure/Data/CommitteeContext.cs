using CommitteeCore.Entities;
using Microsoft.EntityFrameworkCore;

namespace CommitteeInfrastructure.Data
{
    public class CommitteeContext : DbContext
    {
        public CommitteeContext()
        {
        }
        public CommitteeContext(DbContextOptions<CommitteeContext> options) : base(options)
        {

        }

        public DbSet<Committee> Committees { get; set; }
        public DbSet<CommitteeMember> CommitteeMembers { get; set; }
        public DbSet<CommitteeMemberRole> CommitteeMemberRoles { get; set; }
        public DbSet<CommitteeDocument> CommitteeDocuments { get; set; }
        public DbSet<CommitteeDecision> CommitteeDecisions { get; set; }


        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    if (!optionsBuilder.IsConfigured)
        //    {
        //        // Connection string مباشرة → LocalDB
        //        optionsBuilder.UseSqlServer(@"Server=(localdb)\MSSQLLocalDB;Database=CommitteeDb;Trusted_Connection=True;");
        //    }
        //}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // إصلاح تحذير الخاصية Budget
            modelBuilder.Entity<Committee>()
                .Property(c => c.Budget)
                .HasPrecision(18, 2); // <-- يمكنك تعديل الدقة والقياس حسب حاجتك

            modelBuilder.Entity<Committee>()
            .HasIndex(c => c.Name)
            .IsUnique();
        }
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return base.SaveChangesAsync(cancellationToken);
        }
    }

}