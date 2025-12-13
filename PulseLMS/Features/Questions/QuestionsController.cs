using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PulseLMS.Common;
using PulseLMS.Data;
using PulseLMS.Features.Questions.DTO;

namespace PulseLMS.Features.Questions;

public class QuestionsController(AppDbContext dbContext): BaseController
{
    [HttpGet]
    public async Task<IActionResult> GetAllQuestions(CancellationToken ct)
    {
        var questions = await dbContext.Questions
                .AsNoTracking()
                .Include(x => x.Options)
                .OrderBy(x => x.Title)
                .Select(res => new QuestionResponse
                {
                    Id = res.Id,
                    Title = res.Title,
                    Type = res.Type,
                    Description = res.Description,
                    Options = res.Options.Select(op => new QuestionOptionResponse
                    {
                        Id = op.Id,
                        Text = op.Text,
                        SortOrder = op.SortOrder,
                        Score = op.Score
                    })
                })
                .ToListAsync(ct);
        
        return Ok(questions);
    }
}