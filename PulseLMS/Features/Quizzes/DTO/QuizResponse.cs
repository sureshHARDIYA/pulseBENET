using PulseLMS.Domain.Entities;

namespace PulseLMS.Features.Quizzes.DTO;

public class QuizResponse
{
    public Guid Id { get; init; }
    public string Title { get; init; } = null!;
    public string? Description { get; init; }
    public QuizType Type { get; init; }
    public CefrLevel Level { get; init; }
    public List<Guid> CategoryIds { get; init; } = [];
    public AccessType Access { get; init; }
}