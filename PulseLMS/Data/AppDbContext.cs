    using Microsoft.EntityFrameworkCore;
    using PulseLMS.Domain.Entities;

    namespace PulseLMS.Data;

    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<Quiz> Quizzes { get; set; }
        public DbSet<QuizCategory> QuizCategories { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<QuestionOption> QuestionOptions { get; set; }

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
                b.Property(x => x.Access).HasConversion<string>().IsRequired();
                b.Property(x => x.CreatedAt).HasDefaultValueSql("now()");
                b.Property(x => x.UpdatedAt).HasDefaultValueSql("now()");

                // 1: M relations with QuizCategory
                b.HasMany(x => x.QuizCategories)
                    .WithOne(x => x.Quiz)
                    .HasForeignKey(x => x.QuizId)
                    .OnDelete(DeleteBehavior.Cascade);
                
                // M:N relationship with QuizCategory being the Pivot Table
                b.HasMany(q => q.Categories)
                    .WithMany(c => c.Quizzes)
                    .UsingEntity<QuizCategory>();
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
                
                b.Property(x => x.CreatedAt).HasDefaultValueSql("now()");
                b.Property(x => x.UpdatedAt).HasDefaultValueSql("now()");
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

                b.HasOne(x => x.Parent)
                 .WithMany(x => x.Children)
                 .HasForeignKey(x => x.ParentId)
                 .OnDelete(DeleteBehavior.Restrict);

                b.HasIndex(x => x.ParentId);
                
                b.Property(x => x.CreatedAt).HasDefaultValueSql("now()");
                b.Property(x => x.UpdatedAt).HasDefaultValueSql("now()");
            });

            modelBuilder.Entity<Question>(b =>
            {
                b.ToTable("questions");
                
                b.HasKey(x => x.Id);
                
                b.Property(x => x.Id)
                    .HasColumnType("uuid")
                    .HasDefaultValueSql("gen_random_uuid()");
                
                b.Property(x => x.Title)
                    .IsRequired()
                    .HasMaxLength(1000);
                b.Property(x => x.Type)
                    .HasConversion<string>()
                    .IsRequired();
                b.Property(x => x.SortOrder).IsRequired();
                b.Property(x => x.Points).IsRequired();
                b.Property(x => x.AllowMultipleCorrect).IsRequired();
                b.Property(x => x.CreatedAt).HasDefaultValueSql("now()");
                b.Property(x => x.UpdatedAt).HasDefaultValueSql("now()");
                
                b.HasOne(x => x.Quiz)
                    .WithMany(q => q.Questions)
                    .HasForeignKey(x => x.QuizId)
                    .OnDelete(DeleteBehavior.Cascade);

                // one question -> many options, delete question => delete options
                b.HasMany(x => x.Options)
                    .WithOne(o => o.Question)
                    .HasForeignKey(o => o.QuestionId)
                    .OnDelete(DeleteBehavior.Cascade);
                
                b.HasIndex(x => x.QuizId);
                b.HasIndex(x => new { x.QuizId, x.SortOrder });
            });
            
            modelBuilder.Entity<QuestionOption>(b =>
            {
                b.ToTable("question_options");
                
                b.HasKey(x => x.Id);
                
                b.Property(x => x.Id)
                    .HasColumnType("uuid")
                    .HasDefaultValueSql("gen_random_uuid()");
                
                b.Property(x => x.Text).IsRequired().HasMaxLength(1000);
                b.Property(x => x.IsCorrect).IsRequired();
                b.Property(o => o.QuestionId).IsRequired();
                b.Property(x => x.PromptCorrect).HasMaxLength(1000);
                b.Property(x => x.PromptWrong).HasMaxLength(1000);
                b.Property(x => x.Score).IsRequired();
                b.Property(x => x.SortOrder).IsRequired();
                b.Property(x => x.CreatedAt).HasDefaultValueSql("now()");
                b.Property(x => x.UpdatedAt).HasDefaultValueSql("now()");
                
                b.HasOne(o => o.Question)
                    .WithMany(q => q.Options)
                    .HasForeignKey(o => o.QuestionId)
                    .OnDelete(DeleteBehavior.Cascade);
                
                b.HasIndex(o => o.QuestionId);
                b.HasIndex(x => new { x.QuestionId, x.SortOrder });
            });
        }
    }