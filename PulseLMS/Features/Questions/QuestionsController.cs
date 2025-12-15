using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PulseLMS.Common;
using PulseLMS.Data;
using PulseLMS.Domain.Entities;
using PulseLMS.Features.QuestionOptions.DTO;
using PulseLMS.Features.Questions.DTO;

namespace PulseLMS.Features.Questions;

[Route("api/quizzes/{quizId:guid}/questions")]
public class QuestionsController(AppDbContext dbContext): BaseController
{
    [HttpGet]
    [ProducesResponseType(typeof(List<QuestionResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllQuestions([FromRoute] Guid quizId, CancellationToken ct)
    {
        var questions = await dbContext.Questions
                .AsNoTracking()
                .Where(x => x.QuizId == quizId)
                .OrderBy(x => x.Title)
                .Select(QuestionProjection.List)
                .ToListAsync(ct);
        
        return Ok(questions);
    }

    [HttpPost]
    [ProducesResponseType(typeof(QuestionResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateQuestion([FromRoute] Guid quizId, [FromBody] QuestionCreateRequest request, CancellationToken ct)
    {
        var question = new Question
        {
            QuizId = quizId, 
            Title = request.Title.Trim(),
            Description = request.Description?.Trim(),
            Type = request.Type,
            SortOrder = request.SortOrder,
            Points = request.Points,
            AllowMultipleCorrect = request.AllowMultipleCorrect
        };

        dbContext.Questions.Add(question);
        await dbContext.SaveChangesAsync(ct);
        
        var response = await dbContext.Questions
            .AsNoTracking()
            .Where(q => q.Id == question.Id && q.QuizId == quizId)
            .Select(QuestionProjection.Detail)
            .FirstAsync(ct);
        
        return CreatedAtAction(nameof(GetQuestionById), new { quizId, id = response.Id }, response);
    }
    
    [HttpGet("{questionId:guid}")]
    [ProducesResponseType(typeof(QuestionResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetQuestionById([FromRoute] Guid quizId, [FromRoute] Guid questionId, CancellationToken ct)
    {
        var question = await dbContext.Questions
            .AsNoTracking()
            .Where(q => q.Id == questionId && q.QuizId == quizId)
            .Select(QuestionProjection.DetailWithOptions) 
            .FirstOrDefaultAsync(ct);
        
        if (question is null)
            return NotFound();
        
        return Ok(question);
    }

    [HttpPut("{questionId:guid}")]
    [ProducesResponseType(typeof(QuestionResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateQuestion([FromRoute] Guid questionId, [FromRoute] Guid quizId, [FromBody] UpdateQuestionRequest request, CancellationToken ct)
    {
        var question = await dbContext.Questions
            .FirstOrDefaultAsync(q => (q.Id == questionId && q.QuizId == quizId), ct);
        if (question is null)
            return NotFound();


        question.Title = request.Title.Trim();
        question.Description = request.Description?.Trim();
        question.Type = request.Type;
        question.SortOrder = request.SortOrder;
        question.Points = request.Points;
        question.AllowMultipleCorrect = request.AllowMultipleCorrect;
        
        await dbContext.SaveChangesAsync(ct);
        
        var response = await dbContext.Questions
            .AsNoTracking()
            .Where(q => q.Id == questionId && q.QuizId == quizId)
            .Select(QuestionProjection.Detail)
            .FirstAsync(ct);

        return Ok(response);
    }

    [HttpDelete("{questionId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteQuestion([FromRoute] Guid questionId,  [FromRoute] Guid quizId, CancellationToken ct)
    {
        var question = await dbContext.Questions
            .FirstOrDefaultAsync(q => q.Id == questionId && q.QuizId == quizId, ct);     
        if (question is null)
            return NotFound();
        
        dbContext.Questions.Remove(question);
        await dbContext.SaveChangesAsync(ct);
        
        return NoContent();
    }
}