namespace PulseLMS.Features.QuestionOptions.DTO;

public class QuestionOptionResponse
{
    public Guid Id { get; init; }
    public required string Text { get; init; }
    public int SortOrder { get; init; }
    public int Score { get; init; }
    
    public Guid QuestionId { get; init; }
    
    public bool IsCorrect { get; init; }
    
    public string? PromptCorrect { get; init; }
    public string? PromptWrong { get; init; }
}