using PulseLMS.Domain.Entities;

namespace PulseLMS.Features.QuizAttempt.DTO;

public class StartQuizAttemptRequest
{
    public QuizAttemptMode Mode { get; init; }
}