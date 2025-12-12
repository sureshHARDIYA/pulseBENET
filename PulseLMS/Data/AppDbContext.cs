using Microsoft.EntityFrameworkCore;
using PulseLMS.Domain.Entities;

namespace PulseLMS.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Quiz> Quizzes { get; set; }
    public DbSet<QuizCategory> QuizCategories { get; set; }
    public DbSet<Category> Categories { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("public");

        modelBuilder.Entity<Quiz>(b =>
        {
            b.ToTable("quizzes");
            
            b.HasKey(x => x.Id);
            b.Property(x => x.Id)
                .HasColumnType("uuid")
                .HasDefaultValueSql("gen_random_uuid()");
            
            b.Property(x => x.Title).IsRequired().HasMaxLength(200);
            b.Property(x => x.Description).HasMaxLength(1000);

            b.Property(x => x.Type).HasConversion<string>().IsRequired();
            b.Property(x => x.Level).HasConversion<string>().IsRequired();
            b.Property(x => x.Access).HasConversion<string>().IsRequired();

            b.HasMany(x => x.QuizCategories)
                .WithOne(x => x.Quiz)
                .HasForeignKey(x => x.QuizId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<QuizCategory>(b =>
        {
            b.ToTable("quiz_categories");
            b.HasKey(x => new { x.QuizId, x.CategoryId });
            
            // One quiz can be linked to many categories
            // Deleting quiz will delete quiz and unlink with categories
            b.HasOne(x => x.Quiz)
                .WithMany(x => x.QuizCategories)
                .HasForeignKey(x => x.QuizId)
                .OnDelete(DeleteBehavior.Cascade);

            // One category can be linked to many quizzes
            // Will not allow deleting quiz categories if any quiz is using it.
            b.HasOne(x => x.Category)
                .WithMany(x => x.QuizCategories)
                .HasForeignKey(x => x.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);
        });
        
        modelBuilder.Entity<Category>(b =>
        {
            b.ToTable("categories");

            b.HasKey(x => x.Id);

            b.Property(x => x.Id)
                .HasColumnType("uuid")
                .HasDefaultValueSql("gen_random_uuid()");

            b.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(200);

            b.Property(x => x.Description)
                .HasMaxLength(1000);
        });
    }
}