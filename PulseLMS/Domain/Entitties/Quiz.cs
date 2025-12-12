namespace PulseLMS.Domain.Entities;

public enum QuizType { Words, Sentences, Mixed }
public enum CefrLevel { A1, A2, B1, B2, C1, C2 }
public enum AccessType { Free, Paid }

public class Quiz
{
    public Guid Id { get; set; }
    public string Title { get; set; } = null!;
    public string? Description { get; set; }

    public QuizType Type { get; set; }
    public CefrLevel Level { get; set; }
    public AccessType Access { get; set; }

    public List<QuizCategory> QuizCategories { get; set; } = [];
}

public class QuizCategory
{
    public Guid QuizId { get; set; }
    public Quiz Quiz { get; set; } = null!;

    public Guid CategoryId { get; set; }
    public Category Category { get; set; } = null!;
}