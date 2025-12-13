using FluentValidation;
using PulseLMS.Domain.Entities;

namespace PulseLMS.Features.Quizzes.DTO;

public sealed class CreateQuizRequest
{
    public string Title { get; init; } = null!;
    public string? Description { get; init; }
    public QuizType Type { get; init; }
    
    public List<Guid> CategoryIds { get; init; } = [];
    public AccessType Access { get; init; }
}

public class CreateQuizRequestValidator : AbstractValidator<CreateQuizRequest>
{
    public CreateQuizRequestValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Description).MaximumLength(1000);
        RuleFor(x => x.Access).IsInEnum();
        
        RuleFor(x => x.CategoryIds)
            .NotNull();

        RuleForEach(x => x.CategoryIds)
            .NotEmpty();

        RuleFor(x => x.CategoryIds)
            .Must(ids => ids.Distinct().Count() == ids.Count)
            .WithMessage("CategoryIds must be unique.");

    }
}