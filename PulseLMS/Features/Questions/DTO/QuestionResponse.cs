using PulseLMS.Domain.Entities;

namespace PulseLMS.Features.Questions.DTO;

public class QuestionResponse
{
    public Guid Id { get; init; }
    public string? Title { get; init; } = null!;
    public string? Description { get; init; }
    
    public QuestionType Type { get; init; }
    
    public IEnumerable<QuestionOptionResponse> Options { get; set; } = [];
}

public class QuestionOptionResponse
{
    public Guid Id { get; init; }
    public required string Text { get; init; }
    public int SortOrder { get; init; }
    public int Score { get; init; }
}