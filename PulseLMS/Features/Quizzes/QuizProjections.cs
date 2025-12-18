using System.Linq.Expressions;
using PulseLMS.Domain.Entities;
using PulseLMS.Features.Categories.DTO;
using PulseLMS.Features.Quizzes.DTO;

namespace PulseLMS.Features.Quizzes;

public static class QuizProjections
{
    public static readonly Expression<Func<Quiz, QuizResponse>> ToResponse = q => new QuizResponse
    {
        Id = q.Id,
        Title = q.Title,
        Description = q.Description,
        Type = q.Type,
        Access = q.Access,
		CreatedAt = q.CreatedAt,
		UpdatedAt = q.UpdatedAt,
		CreatedBy = q.CreatedBy,
		UpdatedBy = q.UpdatedBy,
        Categories = q.Categories
            .Select(cat => new CategoryResponse()
            {
                Id = cat.Id,
				Name = cat.Name ?? string.Empty,
				CreatedAt = cat.CreatedAt,
				UpdatedAt = cat.UpdatedAt,
				CreatedBy = cat.CreatedBy,
				UpdatedBy = cat.UpdatedBy
            })
            .ToList()
    };
}