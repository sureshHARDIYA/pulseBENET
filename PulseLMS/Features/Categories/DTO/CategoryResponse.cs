namespace PulseLMS.Features.Categories.DTO;

public sealed class CategoryResponse
{
    public Guid Id { get; init; }
    public string? Name { get; init; } = null!;
    public string? Description { get; init; }
    public Guid? ParentId { get; init; }
}

public sealed class CategoryTreeResponse
{
    public Guid Id { get; init; }
    public string Name { get; init; } = null!;
    public string? Description { get; init; }
    public List<CategoryTreeResponse> Children { get; init; } = [];
}