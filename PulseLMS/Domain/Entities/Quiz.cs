namespace PulseLMS.Domain.Entities;

public enum QuizType { Words, Sentences, Mixed }
public enum AccessType { Free, Paid }

public class Quiz: AuditLog
{
    public Guid Id { get; set; }
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public QuizType Type { get; set; }
    public AccessType Access { get; set; }
    
    public List<Category> Categories { get; set; } = [];
    
    public List<Question> Questions { get; set; } = [];
    public List<QuizCategory> QuizCategories { get; set; } = [];

}