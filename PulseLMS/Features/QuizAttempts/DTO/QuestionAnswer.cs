namespace PulseLMS.Features.QuizAttempt.DTO;

public class QuestionAnswer
{
	public required Guid QuestionId { get; init; }
	public required IEnumerable<Guid> SelectedOptionIds { get; init; }
}


