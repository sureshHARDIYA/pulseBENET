namespace PulseLMS.Features.Questions.DTO;

public class CheckQuestionResponse
{
	public required Guid QuestionId { get; init; }
	public required IEnumerable<Guid> SelectedOptionIds { get; init; }
	public required IEnumerable<Guid> CorrectOptionIds { get; init; }
	public bool IsCorrect { get; init; }
	public int AwardedScore { get; init; }
	public int MaxScore { get; init; }
}


