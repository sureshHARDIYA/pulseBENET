using System.Linq.Expressions;
using PulseLMS.Domain.Entities;
using PulseLMS.Features.QuestionOptions;
using PulseLMS.Features.Questions.DTO;

namespace PulseLMS.Features.Questions;

public static class QuestionProjection
{
    public static readonly Expression<Func<Question, QuestionResponse>> List =
        q => new QuestionResponse
        {
            Id = q.Id,
            Title = q.Title,
            Type = q.Type,
            Description = q.Description,
            SortOrder = q.SortOrder,
            Points = q.Points,
            AllowMultipleCorrect = q.AllowMultipleCorrect,
            Options = q.Options
                .AsQueryable()
                .OrderBy(o => o.SortOrder)
                .Select(QuestionOptionProjection.List)
                .ToList()
        };

    public static readonly Expression<Func<Question, QuestionResponse>> Detail =
        q => new QuestionResponse
        {
            Id = q.Id,
            Title = q.Title,
            Type = q.Type,
            Description = q.Description,
            SortOrder = q.SortOrder,
            Points = q.Points,
            AllowMultipleCorrect = q.AllowMultipleCorrect
        };

    public static readonly Expression<Func<Question, QuestionResponse>> DetailWithOptions =
        q => new QuestionResponse
        {
            Id = q.Id,
            Title = q.Title,
            Type = q.Type,
            Description = q.Description,
            SortOrder = q.SortOrder,
            Points = q.Points,
            AllowMultipleCorrect = q.AllowMultipleCorrect,
            Options = q.Options
                .AsQueryable()
                .OrderBy(o => o.SortOrder)
                .Select(QuestionOptionProjection.List)
                .ToList()
        };
}