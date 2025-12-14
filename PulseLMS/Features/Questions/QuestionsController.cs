using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PulseLMS.Common;
using PulseLMS.Data;
using PulseLMS.Domain.Entities;
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
                .Select(res => new QuestionResponse
                {
                    Id = res.Id,
                    Title = res.Title,
                    Type = res.Type,
                    Description = res.Description,
                    SortOrder = res.SortOrder,
                    Points = res.Points,
                    AllowMultipleCorrect = res.AllowMultipleCorrect,
                    Options = res.Options.Select(op => new QuestionOptionResponse
                    {
                        Id = op.Id,
                        Text = op.Text,
                        SortOrder = op.SortOrder,
                        Score = op.Score,
                    })
                })
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
            .Where(q => q.Id == question.Id)
            .Select(res => new QuestionResponse
            {
                Id = res.Id,
                Title = res.Title,
                Type = res.Type,
                Description = res.Description,
                SortOrder = res.SortOrder,
                Points = res.Points,
                AllowMultipleCorrect = res.AllowMultipleCorrect,
            })
            .FirstAsync(ct);
        
        return CreatedAtAction(nameof(GetQuestionById), new { quizId, id = response.Id }, response);
    }
    
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(QuestionResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetQuestionById([FromRoute] Guid quizId, [FromRoute] Guid id, CancellationToken ct)
    {
        var question = await dbContext
            .Questions
            .AsNoTracking()
            .FirstOrDefaultAsync(entity => (entity.Id == id && entity.QuizId == quizId), ct);
        
        if (question is null)
            return NotFound();
        
        return Ok(new QuestionResponse
        {
            Id = question.Id,
            Title = question.Title,
            Type = question.Type,
            Description = question.Description,
            SortOrder = question.SortOrder,
            Points = question.Points,
            AllowMultipleCorrect = question.AllowMultipleCorrect,
        });
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
        
        dbContext.Questions.Update(question);
        await dbContext.SaveChangesAsync(ct);
        
        return Ok(new QuestionResponse
        {
            Id = question.Id,
            Title = question.Title,
            Type = question.Type,
            Description = question.Description,
            SortOrder = question.SortOrder,
            Points = question.Points,
            AllowMultipleCorrect = question.AllowMultipleCorrect,
        });
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteQuestion([FromRoute] Guid id,  [FromRoute] Guid quizId, CancellationToken ct)
    {
        var question = await dbContext.Questions
            .FirstOrDefaultAsync(q => q.Id == id && q.QuizId == quizId, ct);     
        if (question is null)
            return NotFound();
        
        dbContext.Questions.Remove(question);
        await dbContext.SaveChangesAsync(ct);
        
        return NoContent();
    }
}