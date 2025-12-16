namespace PulseLMS.Domain.Entities;

public class QuizAttemptAnswer : AuditLog
{
	public Guid Id { get; set; }
	
	public Guid AttemptId { get; set; }
	
	public Guid QuestionId { get; set; }
	
	public List<Guid> SelectedOptionIds { get; set; } = [];
	
	public int AwardedScore { get; set; }
	
	public bool IsCorrect { get; set; }
	
	public DateTimeOffset AnsweredAt { get; set; } = DateTimeOffset.UtcNow;
}


