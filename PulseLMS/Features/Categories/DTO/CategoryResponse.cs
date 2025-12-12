namespace PulseLMS.Features.Categories.Dtos;

public sealed class CategoryResponse
{
    public Guid Id { get; init; }
    public string Name { get; init; } = null!;
    public string? Description { get; init; }
}