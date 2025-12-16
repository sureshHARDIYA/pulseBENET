namespace PulseLMS.Features.QuizAttempt.DTO;

public class SaveAnswerResponse
{
	public required Guid AttemptId { get; init; }
	public required Guid QuestionId { get; init; }
	public required IEnumerable<Guid> SelectedOptionIds { get; init; }
	public bool IsCorrect { get; init; }
	public int AwardedScore { get; init; }
	public int QuestionMaxScore { get; init; }
	public int AttemptScore { get; init; }
	public int AnsweredQuestions { get; init; }
	public int TotalQuestions { get; init; }
}


