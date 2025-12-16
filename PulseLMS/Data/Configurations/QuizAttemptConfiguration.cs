using PulseLMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace PulseLMS.Data.Configurations;

public sealed class QuizAttemptConfiguration : IEntityTypeConfiguration<QuizAttempt>
{
    public void Configure(EntityTypeBuilder<QuizAttempt> b)
    {
        b.ToTable("quiz_attempts");
        b.HasKey(x => x.Id);

        b.Property(x => x.Mode).HasConversion<int>();
        b.Property(x => x.Status).HasConversion<int>();

        b.HasIndex(x => new { x.QuizId, x.UserId, x.Status });
        b.HasIndex(x => x.UserId);

        b.Property(x => x.StartedAt).HasDefaultValueSql("now()");
        b.Property(x => x.LastActivityAt).HasDefaultValueSql("now()");
        
        b.Property(x => x.CreatedAt).HasDefaultValueSql("now()");
        b.Property(x => x.UpdatedAt).HasDefaultValueSql("now()");

        b.HasOne(x => x.Quiz)
            .WithMany() // or .WithMany(q => q.Attempts) if you add navigation
            .HasForeignKey(x => x.QuizId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}