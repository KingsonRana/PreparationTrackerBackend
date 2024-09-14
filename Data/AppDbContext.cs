using Microsoft.EntityFrameworkCore;
using PreparationTracker.Model;

namespace PreparationTracker.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {   
        }

        public DbSet<Topic> Topics { get; set; }
        public DbSet<Problems> Problems { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuring the one-to-many relationship
            modelBuilder.Entity<Topic>()
                .HasMany(t => t.Problems)
                .WithOne(p => p.Topic)
                .HasForeignKey(p => p.TopicGuid)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Topic>()
                .HasOne(t => t.Parent)
                .WithMany(t => t.SubTopics)
                .HasForeignKey(t => t.ParentId)
                .OnDelete(DeleteBehavior.Restrict); // Use Restrict instead of Cascade


            modelBuilder.Entity<Topic>()
                .HasIndex(t => t.Name)
                .IsUnique();
        }

    }
}
