using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PulseLMS.Common;
using PulseLMS.Data;
using PulseLMS.Domain.Entities;
using PulseLMS.Features.QuestionOptions.DTO;

namespace PulseLMS.Features.QuestionOptions;

[Route("api/quizzes/{quizId:guid}/questions/{questionId:guid}/options")]
public sealed class QuestionOptionsController(AppDbContext dbContext): BaseController
{
    [HttpGet]
    [ProducesResponseType(typeof(List<QuestionOptionResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAllQuestionOptions([FromRoute] Guid quizId, [FromRoute] Guid questionId, CancellationToken ct)
    {
        var questionExists = await dbContext.Questions
            .AsNoTracking()
            .AnyAsync(q => q.Id == questionId && q.QuizId == quizId, ct);

        if (!questionExists)
            return NotFound();
        
        var options = await dbContext.QuestionOptions
            .AsNoTracking()
            .Where(o => o.QuestionId == questionId)
            .OrderBy(o => o.SortOrder) 
            .Select(QuestionOptionProjection.List)
            .ToListAsync(ct);

        return Ok(options);
    }

    [HttpPost]
    [ProducesResponseType(typeof(QuestionOptionResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CreateQuestionOption(
        [FromRoute] Guid quizId,
        [FromRoute] Guid questionId,
        [FromBody] QuestionOptionRequest request,
        CancellationToken ct)
    {
        var exists = await dbContext.Questions
            .AsNoTracking()
            .AnyAsync(q => q.Id == questionId && q.QuizId == quizId, ct);

        if (!exists)
            return NotFound();

        var option = new QuestionOption
        {
            QuestionId = questionId,
            Text = request.Text.Trim(),
            SortOrder = request.SortOrder,
            Score = request.Score,
            IsCorrect = request.IsCorrect,
            PromptCorrect = request.PromptCorrect?.Trim(),
            PromptWrong = request.PromptWrong?.Trim()
        };

        dbContext.QuestionOptions.Add(option);
        await dbContext.SaveChangesAsync(ct);

        var response = new QuestionOptionResponse
        {
            Id = option.Id,
            QuestionId = option.QuestionId,
            Text = option.Text,
            SortOrder = option.SortOrder,
            Score = option.Score,
            IsCorrect = option.IsCorrect,
            PromptCorrect = option.PromptCorrect,
            PromptWrong = option.PromptWrong
        };

        return CreatedAtAction(
            nameof(GetQuestionOptionById),
            new { quizId, questionId, id = option.Id },
            response);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(QuestionOptionResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetQuestionOptionById(
        [FromRoute] Guid quizId,
        [FromRoute] Guid questionId,
        [FromRoute] Guid id,
        CancellationToken ct)
    {
        var option = await dbContext.QuestionOptions
            .AsNoTracking()
            .Where(o => o.Id == id &&
                        o.QuestionId == questionId &&
                        o.Question.QuizId == quizId)
            .Select(QuestionOptionProjection.Detail)
            .FirstOrDefaultAsync(ct);

        if (option is null)
            return NotFound();

        return Ok(option);
    }
	
	[HttpPut("{id:guid}")]
	[ProducesResponseType(typeof(QuestionOptionResponse), StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<IActionResult> UpdateQuestionOption(
		[FromRoute] Guid quizId,
		[FromRoute] Guid questionId,
		[FromRoute] Guid id,
		[FromBody] QuestionOptionRequest request,
		CancellationToken ct)
	{
		var option = await dbContext.QuestionOptions
			.Include(o => o.Question)
			.FirstOrDefaultAsync(
				o => o.Id == id &&
				     o.QuestionId == questionId &&
				     o.Question.QuizId == quizId,
				ct);
		
		if (option is null)
			return NotFound();
		
		option.Text = request.Text.Trim();
		option.SortOrder = request.SortOrder;
		option.Score = request.Score;
		option.IsCorrect = request.IsCorrect;
		option.PromptCorrect = string.IsNullOrWhiteSpace(request.PromptCorrect) ? null : request.PromptCorrect.Trim();
		option.PromptWrong = string.IsNullOrWhiteSpace(request.PromptWrong) ? null : request.PromptWrong.Trim();
		
		await dbContext.SaveChangesAsync(ct);
		
		var response = await dbContext.QuestionOptions
			.AsNoTracking()
			.Where(o => o.Id == id)
			.Select(QuestionOptionProjection.Detail)
			.FirstAsync(ct);
		
		return Ok(response);
	}
    
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteQuestionOption(
        [FromRoute] Guid quizId,
        [FromRoute] Guid questionId,
        [FromRoute] Guid id,
        CancellationToken ct)
    {
        var exists = await dbContext.QuestionOptions
            .AnyAsync(
                o => o.Id == id &&
                     o.QuestionId == questionId &&
                     o.Question.QuizId == quizId,
                ct);

        if (!exists)
            return NotFound();

        dbContext.QuestionOptions.Remove(new QuestionOption { Id = id });
        await dbContext.SaveChangesAsync(ct);

        return NoContent();
    }
}