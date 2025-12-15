using FluentValidation;
using PulseLMS.Domain.Entities;

namespace PulseLMS.Features.Questions.DTO;

public sealed class QuestionCreateRequest
{
    public required string Title { get; init; }
    public string? Description { get; init; }
    public QuestionType Type { get; init; }
    public int SortOrder { get; init; } = 0;
    
    public int Points { get; init; }
    
    public bool AllowMultipleCorrect { get; init; }
}

public class QuestionCreateRequestValidator
    : AbstractValidator<QuestionCreateRequest>
{
    public QuestionCreateRequestValidator()
    {
        this.ApplyCommonRules(
            x => x.Title,
            x => x.Description,
            x => x.Type,
            x => x.SortOrder,
            x => x.Points,
            x => x.AllowMultipleCorrect
        );
    }
}
