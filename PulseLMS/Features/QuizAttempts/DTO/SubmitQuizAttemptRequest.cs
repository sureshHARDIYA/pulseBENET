namespace PulseLMS.Features.QuizAttempt.DTO;

public class SubmitQuizAttemptRequest
{
	public required IEnumerable<QuestionAnswer> Answers { get; init; }
}


