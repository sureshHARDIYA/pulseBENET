namespace PulseLMS.Features.QuizAttempt.DTO;

public class SaveAnswerRequest
{
	public required IEnumerable<Guid> SelectedOptionIds { get; init; }
}


