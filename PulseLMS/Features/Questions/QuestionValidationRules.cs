using FluentValidation;
using PulseLMS.Domain.Entities;

namespace PulseLMS.Features.Questions;

public static class QuestionValidationRules
{
    public static void ApplyCommonRules<T>(
        this AbstractValidator<T> validator, 
        Func<T, string> title,
        Func<T, string?> description,
        Func<T, QuestionType> type,
        Func<T, int> sortOrder,
        Func<T, int> points,
        Func<T, bool> allowMultipleCorrect)
    {
        validator.RuleFor(x => title(x))
            .NotEmpty()
            .MaximumLength(200)
            .Must(t => t.Trim().Length > 0);

        validator.RuleFor(x => description(x))
            .MaximumLength(1000);

        validator.RuleFor(x => type(x))
            .IsInEnum();

        validator.RuleFor(x => sortOrder(x))
            .GreaterThanOrEqualTo(0);

        validator.RuleFor(x => points(x))
            .GreaterThanOrEqualTo(0);

        validator.RuleFor(x => allowMultipleCorrect(x))
            .Equal(false)
            .When(x => type(x) == QuestionType.TrueFalse)
            .WithMessage("True/False questions cannot have multiple correct answers.");
    }
}