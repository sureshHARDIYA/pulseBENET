using System.Linq.Expressions;
using PulseLMS.Domain.Entities;
using PulseLMS.Features.QuestionOptions.DTO;

namespace PulseLMS.Features.QuestionOptions;

public static class QuestionOptionProjection
{
    public static readonly Expression<Func<QuestionOption, QuestionOptionResponse>> List =
        o => new QuestionOptionResponse
        {
            Id = o.Id,
            Text = o.Text,
            SortOrder = o.SortOrder,
            Score = o.Score,
            PromptCorrect = o.PromptCorrect,
            PromptWrong = o.PromptWrong,
            IsCorrect = o.IsCorrect
        };

    public static readonly Expression<Func<QuestionOption, QuestionOptionResponse>> Detail =
        o => new QuestionOptionResponse
        {
            Id = o.Id,
            QuestionId = o.QuestionId,
            Text = o.Text,
            SortOrder = o.SortOrder,
            Score = o.Score,
            IsCorrect = o.IsCorrect,
            PromptCorrect = o.PromptCorrect,
            PromptWrong = o.PromptWrong
        };
}