using FluentValidation;

namespace PulseLMS.Features.Categories.DTO;

public sealed class CategoryRequest
{
    public string Name { get; init; } = null!;
    public string? Description { get; init; }
    public Guid? ParentId { get; init; }
}

public class CategoryRequestValidator : AbstractValidator<CategoryRequest>
{
    public CategoryRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Description)
            .MaximumLength(1000);
    }
}