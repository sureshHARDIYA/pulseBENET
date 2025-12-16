using PulseLMS.Domain.Entities;

namespace PulseLMS.Features.QuizAttempt.DTO;

public class QuizAttemptResponse
{
    public Guid Id { get; init; }
    public Guid QuizId { get; init; }
    public Guid? UserId { get; init; }
    public QuizAttemptMode Mode { get; init; }
    public QuizAttemptStatus Status { get; init; }
    public DateTimeOffset StartedAt { get; init; }
    public DateTimeOffset LastActivityAt { get; init; }
    public int TotalQuestions { get; init; }
    public int AnsweredQuestions { get; init; }
    public int MaxScore { get; init; }
    public int Score { get; init; }
}