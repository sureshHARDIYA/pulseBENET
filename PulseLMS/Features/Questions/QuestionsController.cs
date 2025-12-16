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
	[HttpPost("{questionId:guid}/check")]
	[ProducesResponseType(typeof(CheckQuestionResponse), StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<IActionResult> CheckQuestion([FromRoute] Guid quizId, [FromRoute] Guid questionId, [FromBody] CheckQuestionRequest request, CancellationToken ct)
	{
		var question = await dbContext.Questions
			.AsNoTracking()
			.Include(q => q.Options)
			.FirstOrDefaultAsync(q => q.Id == questionId && q.QuizId == quizId, ct);
		
		if (question is null)
			return NotFound();

		var selected = request.SelectedOptionIds?.ToHashSet() ?? [];
		var correct = question.Options.Where(o => o.IsCorrect).Select(o => o.Id).ToHashSet();

		int awardedScore = 0;
		bool isCorrect = false;

		// Only option-based questions are supported for checking
		if (question.Type is QuestionType.MultipleChoice or QuestionType.TrueFalse)
		{
			if (question.AllowMultipleCorrect)
			{
				// Partial credit: sum scores for correctly selected options, cap at question points
				awardedScore = question.Options
					.Where(o => o.IsCorrect && selected.Contains(o.Id))
					.Sum(o => o.Score);
				if (awardedScore > question.Points) awardedScore = question.Points;
				isCorrect = selected.SetEquals(correct);
			}
			else
			{
				// Single correct: must select exactly one and it must be a correct one
				if (selected.Count == 1 && correct.Overlaps(selected))
				{
					awardedScore = question.Points;
					isCorrect = true;
				}
			}
		}

		var response = new CheckQuestionResponse
		{
			QuestionId = question.Id,
			SelectedOptionIds = selected,
			CorrectOptionIds = correct,
			IsCorrect = isCorrect,
			AwardedScore = awardedScore,
			MaxScore = question.Points
		};
		
		return Ok(response);
	}

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