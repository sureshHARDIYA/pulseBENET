using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PulseLMS.Common;
using PulseLMS.Data;
using PulseLMS.Domain.Entities;
using PulseLMS.Features.Categories.DTO;
using PulseLMS.Features.Quizzes.DTO;

namespace PulseLMS.Features.Quizzes;
public sealed class QuizzesController(AppDbContext dbContext) : BaseController
{
    [HttpGet]
    [ProducesResponseType(typeof(List<QuizResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllQuizzes(CancellationToken ct)
    {
        var quizzes = await dbContext.Quizzes
            .AsNoTracking()
            .OrderBy(q => q.Title)
            .Select(QuizProjections.ToResponse)
            .ToListAsync(ct);

        return Ok(quizzes);
    }

    [HttpPost]
    public async Task<IActionResult> CreateQuiz([FromBody] CreateQuizRequest request, CancellationToken ct)
    {
        var categoryIds = request.CategoryIds.Distinct().ToList();

        if (categoryIds.Count > 0)
        {
            var existingCategoryIds = await dbContext.Categories
                .Where(c => categoryIds.Contains(c.Id))
                .Select(c => c.Id)
                .ToListAsync(ct);

            if (existingCategoryIds.Count != categoryIds.Count)
                return BadRequest("One or more categoryIds do not exist.");
        }

        var quiz = new Quiz
        {
            Title = request.Title.Trim(),
            Description = request.Description?.Trim(),
            Type = request.Type,
            Access = request.Access
        };

        foreach (var categoryId in categoryIds)
        {
            quiz.QuizCategories.Add(new QuizCategory { CategoryId = categoryId });
        }

        dbContext.Quizzes.Add(quiz);
        await dbContext.SaveChangesAsync(ct);

        return CreatedAtAction(nameof(GetQuizById), new { id = quiz.Id }, null);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(QuizResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetQuizById(Guid id, CancellationToken ct)
    {
        var quiz = await dbContext.Quizzes
            .AsNoTracking()
            .Where(q => q.Id == id)
            .Select(QuizProjections.ToResponse)
            .FirstOrDefaultAsync(ct);

        return quiz is null ? NotFound() : Ok(quiz);
    }
}
