namespace PulseLMS.Domain.Entities;

public class QuizAttempt : AuditLog
{
    public Guid Id { get; set; }
    public Guid QuizId { get; set; }
    public Quiz Quiz { get; set; } = null!;

    public Guid? UserId { get; set; }

    public QuizAttemptMode Mode { get; set; }
    public QuizAttemptStatus Status { get; set; } = QuizAttemptStatus.InProgress;

    public DateTimeOffset StartedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? SubmittedAt { get; set; }
    public DateTimeOffset LastActivityAt { get; set; } = DateTimeOffset.UtcNow;

    public int QuizVersion { get; set; } = 1;

    public int TotalQuestions { get; set; }
    public int AnsweredQuestions { get; set; }
    public int MaxScore { get; set; }
    public int Score { get; set; }

    public int? TimeLimitSeconds { get; set; }
    public DateTimeOffset? ExpiresAt { get; set; }
}