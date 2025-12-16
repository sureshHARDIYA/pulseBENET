namespace PulseLMS.Features.QuizAttempts;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using PulseLMS.Common;
using PulseLMS.Data;
using PulseLMS.Features.QuizAttempt.DTO;
using PulseLMS.Domain.Entities;

public class QuizAttemptController(AppDbContext db, ICurrentUser currentUser): BaseController
{
	[HttpPost("attempts/{id:guid}/answers/{questionId:guid}")]
	[Authorize]
	public async Task<IActionResult> SaveAnswer(Guid id, Guid questionId, SaveAnswerRequest request, CancellationToken ct)
	{
		var userId = currentUser.UserId;
		
		var attempt = await db.QuizAttempts
			.FirstOrDefaultAsync(a => a.Id == id && a.UserId == userId && a.Status == QuizAttemptStatus.InProgress, ct);
		if (attempt is null)
			return NotFound();
		
		var question = await db.Questions
			.Include(q => q.Options)
			.FirstOrDefaultAsync(q => q.Id == questionId && q.QuizId == attempt.QuizId, ct);
		if (question is null)
			return NotFound();

		var selected = request.SelectedOptionIds?.ToHashSet() ?? [];
		var correctIds = question.Options.Where(o => o.IsCorrect).Select(o => o.Id).ToHashSet();
		
		int awarded = 0;
		bool isCorrect = false;
		
		if (question.Type is QuestionType.MultipleChoice or QuestionType.TrueFalse)
		{
			if (question.AllowMultipleCorrect)
			{
				awarded = question.Options
					.Where(o => o.IsCorrect && selected.Contains(o.Id))
					.Sum(o => o.Score);
				if (awarded > question.Points) awarded = question.Points;
				isCorrect = selected.SetEquals(correctIds);
			}
			else
			{
				if (selected.Count == 1 && correctIds.Overlaps(selected))
				{
					awarded = question.Points;
					isCorrect = true;
				}
			}
		}

		var existing = await db.QuizAttemptAnswers
			.FirstOrDefaultAsync(a => a.AttemptId == attempt.Id && a.QuestionId == question.Id, ct);
		
		if (existing is null)
		{
			existing = new QuizAttemptAnswer
			{
				Id = Guid.NewGuid(),
				AttemptId = attempt.Id,
				QuestionId = question.Id,
				SelectedOptionIds = selected.ToList(),
				AwardedScore = awarded,
				IsCorrect = isCorrect,
				AnsweredAt = DateTimeOffset.UtcNow
			};
			db.QuizAttemptAnswers.Add(existing);
		}
		else
		{
			existing.SelectedOptionIds = selected.ToList();
			existing.AwardedScore = awarded;
			existing.IsCorrect = isCorrect;
			existing.AnsweredAt = DateTimeOffset.UtcNow;
		}

		// update attempt aggregates from persisted answers
		attempt.Score = await db.QuizAttemptAnswers
			.Where(a => a.AttemptId == attempt.Id)
			.SumAsync(a => (int?)a.AwardedScore, ct) ?? 0;
		
		attempt.AnsweredQuestions = await db.QuizAttemptAnswers
			.Where(a => a.AttemptId == attempt.Id)
			.CountAsync(ct);
		
		attempt.LastActivityAt = DateTimeOffset.UtcNow;

		await db.SaveChangesAsync(ct);

		return Ok(new SaveAnswerResponse
		{
			AttemptId = attempt.Id,
			QuestionId = question.Id,
			SelectedOptionIds = existing.SelectedOptionIds,
			IsCorrect = isCorrect,
			AwardedScore = awarded,
			QuestionMaxScore = question.Points,
			AttemptScore = attempt.Score,
			AnsweredQuestions = attempt.AnsweredQuestions,
			TotalQuestions = attempt.TotalQuestions
		});
	}

	[HttpPost("attempts/{id:guid}/submit")]
	[Authorize]
	public async Task<IActionResult> SubmitAttempt(Guid id, SubmitQuizAttemptRequest request, CancellationToken ct)
	{
		var userId = currentUser.UserId;
		
		var attempt = await db.QuizAttempts
			.FirstOrDefaultAsync(a => a.Id == id && a.UserId == userId && a.Status == QuizAttemptStatus.InProgress, ct);
		if (attempt is null)
			return NotFound();
		
		// Load all questions with options for the quiz
		var quizQuestions = await db.Questions
			.AsNoTracking()
			.Where(q => q.QuizId == attempt.QuizId)
			.Include(q => q.Options)
			.ToListAsync(ct);

		var questionById = quizQuestions.ToDictionary(q => q.Id);

		var requestAnswers = request?.Answers ?? Array.Empty<QuestionAnswer>();

		// if no answers were provided in the submission, fallback to persisted answers
		if (!requestAnswers.Any())
		{
			var persisted = await db.QuizAttemptAnswers
				.AsNoTracking()
				.Where(a => a.AttemptId == attempt.Id)
				.ToListAsync(ct);
			
			requestAnswers = persisted.Select(a => new QuestionAnswer
			{
				QuestionId = a.QuestionId,
				SelectedOptionIds = a.SelectedOptionIds
			}).ToArray();
		}

		var answersByQuestion = requestAnswers
			.GroupBy(a => a.QuestionId)
			.ToDictionary(g => g.Key, g => g.First());

		int totalScore = 0;
		int answeredCount = 0;

		foreach (var (questionId, answer) in answersByQuestion)
		{
			if (!questionById.TryGetValue(questionId, out var question))
				continue; // ignore answers to questions not in this quiz
			
			var selected = answer.SelectedOptionIds?.ToHashSet() ?? [];
			if (selected.Count > 0) answeredCount++;

			int awarded = 0;

			if (question.Type is QuestionType.MultipleChoice or QuestionType.TrueFalse)
			{
				var correctIds = question.Options.Where(o => o.IsCorrect).Select(o => o.Id).ToHashSet();
				if (question.AllowMultipleCorrect)
				{
					awarded = question.Options
						.Where(o => o.IsCorrect && selected.Contains(o.Id))
						.Sum(o => o.Score);
					if (awarded > question.Points) awarded = question.Points;
				}
				else
				{
					if (selected.Count == 1 && correctIds.Overlaps(selected))
						awarded = question.Points;
				}
			}

			totalScore += awarded;
		}
		
		if (totalScore > attempt.MaxScore) totalScore = attempt.MaxScore;

		attempt.Score = totalScore;
		attempt.AnsweredQuestions = answeredCount;
		attempt.Status = QuizAttemptStatus.Submitted;
		attempt.SubmittedAt = DateTimeOffset.UtcNow;
		attempt.LastActivityAt = DateTimeOffset.UtcNow;

		await db.SaveChangesAsync(ct);

		return Ok(Map(attempt));
	}

	[HttpGet("leaderboard")]
	[Authorize]
	public async Task<IActionResult> GetLeaderboard([FromQuery] int limit = 50, CancellationToken ct = default)
	{
		limit = Math.Clamp(limit, 1, 200);
		
		var top = await db.QuizAttempts
			.AsNoTracking()
			.Where(a => a.Status == QuizAttemptStatus.Submitted && a.UserId != null)
			.GroupBy(a => a.UserId!)
			.Select(g => new LeaderboardEntryResponse
			{
				UserId = g.Key,
				TotalScore = g.Sum(a => a.Score),
				Attempts = g.Count(),
				AverageScore = g.Average(a => a.Score)
			})
			.OrderByDescending(x => x.TotalScore)
			.ThenByDescending(x => x.Attempts)
			.Take(limit)
			.ToListAsync(ct);
		
		return Ok(top);
	}

	[HttpGet("stats/me")]
	[Authorize]
	public async Task<IActionResult> GetMyStats(CancellationToken ct)
	{
		var userId = currentUser.UserId;
		var mine = await db.QuizAttempts
			.AsNoTracking()
			.Where(a => a.UserId == userId && a.Status == QuizAttemptStatus.Submitted)
			.ToListAsync(ct);

		var totalQuizzes = mine.Count;
		var totalScore = mine.Sum(a => a.Score);
		var avg = totalQuizzes > 0 ? mine.Average(a => a.Score) : 0.0;

		return Ok(new MyStatsResponse
		{
			TotalQuizzesTaken = totalQuizzes,
			TotalScore = totalScore,
			AverageScore = avg
		});
	}

    [HttpPost("{quizId:guid}/attempts")]
    [Authorize]
    public async Task<IActionResult> StartAttempt(Guid quizId, StartQuizAttemptRequest request, CancellationToken ct)
    {
        var userId = currentUser.UserId; 

        var existing = await db.QuizAttempts
            .AsNoTracking()
            .Where(a => a.QuizId == quizId && a.UserId == userId && a.Status == QuizAttemptStatus.InProgress && a.Mode == request.Mode)
            .OrderByDescending(a => a.StartedAt)
            .FirstOrDefaultAsync(ct);

        if (existing is not null)
            return Ok(Map(existing));

        var totalQuestions = await db.Questions.CountAsync(q => q.QuizId == quizId, ct);
        var maxScore = await db.Questions.Where(q => q.QuizId == quizId).SumAsync(q => q.Points, ct);

        var attempt = new QuizAttempt
        {
            Id = Guid.NewGuid(),
            QuizId = quizId,
            UserId = userId,
            Mode = request.Mode,
            Status = QuizAttemptStatus.InProgress,
            StartedAt = DateTimeOffset.UtcNow,
            LastActivityAt = DateTimeOffset.UtcNow,
            TotalQuestions = totalQuestions,
            MaxScore = maxScore,
            QuizVersion = 1
        };

        db.QuizAttempts.Add(attempt);
        await db.SaveChangesAsync(ct);

        return CreatedAtAction(nameof(GetAttempt), new { id = attempt.Id }, Map(attempt));
    }

    [HttpGet("attempts/{id:guid}")]
    [Authorize]
    public async Task<IActionResult> GetAttempt(Guid id, CancellationToken ct)
    {
        var userId = currentUser.UserId;

        var attempt = await db.QuizAttempts
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == id && a.UserId == userId, ct);

        if (attempt is null) return NotFound();

        return Ok(Map(attempt));
    }

    private static QuizAttemptResponse Map(QuizAttempt a) => new()
    {
        Id = a.Id,
        QuizId = a.QuizId,
        UserId = a.UserId,
        Mode = a.Mode,
        Status = a.Status,
        StartedAt = a.StartedAt,
        LastActivityAt = a.LastActivityAt,
        TotalQuestions = a.TotalQuestions,
        AnsweredQuestions = a.AnsweredQuestions,
        MaxScore = a.MaxScore,
        Score = a.Score
    };
}