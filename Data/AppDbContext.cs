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
        public DbSet<Exam> Exam { get; set; }
        public DbSet<User> Users { get; set; }
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

            modelBuilder.Entity<Topic>()
                .HasOne(t => t.Exam)
                .WithMany(e => e.Topics)
                .HasForeignKey(t => t.ExamId);

            modelBuilder.Entity<Exam>()
                .HasIndex(e => e.ExamName)
                .IsUnique();

            modelBuilder.Entity<User>()
              .HasIndex(u => u.Email)
              .IsUnique(); // Enforcing unique constraint on Email

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Phone)
                .IsUnique(); // Enforcing unique constraint on Phone

            modelBuilder.Entity<User>()
                .HasMany(u => u.Exams)
                .WithOne(e => e.User)
                .HasForeignKey(e => e.UserId); // Cascade delete - deleting user will delete their exams

        }

    }
}
