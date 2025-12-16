using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PulseLMS.Domain.Entities;

namespace PulseLMS.Data.Configurations;

public sealed class QuizAttemptAnswerConfiguration : IEntityTypeConfiguration<QuizAttemptAnswer>
{
	public void Configure(EntityTypeBuilder<QuizAttemptAnswer> b)
	{
		b.ToTable("quiz_attempt_answers");
		
		b.HasKey(x => x.Id);
		
		b.Property(x => x.Id)
			.HasColumnType("uuid")
			.HasDefaultValueSql("gen_random_uuid()");
		
		b.Property(x => x.AttemptId).IsRequired();
	b.Property(x => x.QuestionId).IsRequired();
		
		b.Property(x => x.SelectedOptionIds)
			.HasColumnType("uuid[]")
			.IsRequired();
		
		b.Property(x => x.AwardedScore).IsRequired();
		b.Property(x => x.IsCorrect).IsRequired();
		b.Property(x => x.AnsweredAt).HasDefaultValueSql("now()");
		
		b.Property(x => x.CreatedAt).HasDefaultValueSql("now()");
		b.Property(x => x.UpdatedAt).HasDefaultValueSql("now()");
		
		b.HasIndex(x => new { x.AttemptId, x.QuestionId }).IsUnique();
		b.HasIndex(x => x.AttemptId);
		b.HasIndex(x => x.QuestionId);
	}
}


