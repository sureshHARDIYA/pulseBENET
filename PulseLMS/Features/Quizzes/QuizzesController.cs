using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PulseLMS.Common;
using PulseLMS.Data;
using PulseLMS.Domain.Entities;
using PulseLMS.Features.Categories.Services;
using PulseLMS.Features.Quizzes.DTO;

namespace PulseLMS.Features.Quizzes;
public sealed class QuizzesController(AppDbContext dbContext, CategoryService categoryService) : BaseController
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
    [ProducesResponseType(typeof(QuizResponse),StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateQuiz([FromBody] CreateQuizRequest request, CancellationToken ct)
    {
        var categoryIds = request.CategoryIds.Distinct().ToList();

        var doesAllCategoryExist = await categoryService.AllCategoriesExistAsync(categoryIds, ct);
        
        if (!doesAllCategoryExist)
            return BadRequest("One or more categoryIds do not exist.");

        var quiz = new Quiz
        {
            Title = request.Title.Trim(),
            Description = request.Description?.Trim(),
            Type = request.Type,
            Access = request.Access
        };

        AssignQuizToCategory(categoryIds, quiz);

        dbContext.Quizzes.Add(quiz);
        await dbContext.SaveChangesAsync(ct);

        var response = await dbContext.Quizzes
            .AsNoTracking()
            .Where(q => q.Id == quiz.Id)
            .Select(QuizProjections.ToResponse)
            .FirstAsync(ct);

        return CreatedAtAction(nameof(GetQuizById), new { id = quiz.Id }, response);
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

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(QuizResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateQuiz([FromRoute] Guid id, [FromBody] UpdateQuizRequest request,  CancellationToken ct)
    {
        var targetQuiz = await dbContext.Quizzes
            .Include(q => q.QuizCategories)
            .FirstOrDefaultAsync(q => q.Id == id, ct);

        if (targetQuiz is null)
        {
            return NotFound();
        }

        var categoryIds = request.CategoryIds.Distinct().ToList();
        
        var doesAllCategoryExist =  await categoryService.AllCategoriesExistAsync(categoryIds, ct);

        if (!doesAllCategoryExist)
                return BadRequest("One or more categoryIds do not exist.");
        
        targetQuiz.Title = request.Title.Trim();
        targetQuiz.Description = request.Description?.Trim();
        targetQuiz.Type = request.Type;
        targetQuiz.Access = request.Access;
        
        targetQuiz.QuizCategories.Clear();
        AssignQuizToCategory(categoryIds, targetQuiz);
        
        await dbContext.SaveChangesAsync(ct);
        
        var response = await dbContext.Quizzes
            .AsNoTracking()
            .Where(q => q.Id == id)
            .Select(QuizProjections.ToResponse)
            .FirstAsync(ct);
        
        return Ok(response);
    }

    private static void AssignQuizToCategory(IReadOnlyCollection<Guid> categoryIds, Quiz quiz)
    {
        foreach (var categoryId in categoryIds)
        {
            quiz.QuizCategories.Add(new QuizCategory
            {
                CategoryId = categoryId
            });
        }
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteQuiz(Guid id, CancellationToken ct)
    {
        var deleted = await dbContext.Quizzes
            .Where(q => q.Id == id)
            .ExecuteDeleteAsync(ct);

        return deleted == 0 ? NotFound() : NoContent();
    }
}
