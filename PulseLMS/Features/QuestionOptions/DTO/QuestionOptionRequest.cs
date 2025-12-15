using PulseLMS.Domain.Entities;

namespace PulseLMS.Features.QuestionOptions.DTO;

public class QuestionOptionRequest
{
    public int SortOrder { get; init; }
    public required string Text { get; init; }
    public string? PromptCorrect { get; init; }
    public string? PromptWrong { get; init; }
    public bool IsCorrect { get; init; }
    public int Score { get; init; }
}