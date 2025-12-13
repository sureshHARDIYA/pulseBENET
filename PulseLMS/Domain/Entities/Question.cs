namespace PulseLMS.Domain.Entities;

public enum QuestionType
{
    MultipleChoice,
    TrueFalse,
    FillInTheBlank,
    ShortAnswer
}

public class Question: AuditLog
{
    public Guid Id { get; set; }

    public Guid QuizId { get; set; }
    public Quiz Quiz { get; set; } = null!;

    public string Title { get; set; } = null!;
    public string? Description { get; set; }

    public QuestionType Type { get; set; }
    
    public int SortOrder { get; set; }
    
    public int Points { get; set; }
    
    public bool AllowMultipleCorrect { get; set; }

    public List<QuestionOption> Options { get; set; } = [];
}

public class QuestionOption: AuditLog
{
    public Guid Id { get; set; }

    public Guid QuestionId { get; set; }
    public Question Question { get; set; } = null!;

    public int SortOrder { get; set; }
    
    public string Text { get; set; } = null!;
    public string? PromptCorrect { get; set; }
    public string? PromptWrong { get; set; }

    public bool IsCorrect { get; set; }
    public int Score { get; set; }
}