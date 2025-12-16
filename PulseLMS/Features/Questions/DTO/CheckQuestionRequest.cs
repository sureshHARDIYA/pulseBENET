namespace PulseLMS.Features.Questions.DTO;

public class CheckQuestionRequest
{
	public required IEnumerable<Guid> SelectedOptionIds { get; init; }
}


