using PulseLMS.Common;
using PulseLMS.Domain.Entities;
using PulseLMS.Features.Categories.DTO;

namespace PulseLMS.Features.Quizzes.DTO;

public sealed class QuizResponse : AuditLogResponse
{
	public Guid Id { get; init; }
	public string Title { get; init; } = null!;
	public string? Description { get; init; }
	public QuizType Type { get; init; }
	public AccessType Access { get; init; }
	
	public IEnumerable<CategoryResponse> Categories { get; set; } = [];
}