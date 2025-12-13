namespace PulseLMS.Domain.Entities;

public class Category
{
    public Guid Id { get; set; }
    public string? Name { get; set; } = null!;
    public string? Description { get; set; }


    public Guid? ParentId { get; set; }
    public Category? Parent { get; set; }
    public List<Category> Children { get; set; } = [];

    public List<Quiz> Quizzes { get; set; } = [];
    public List<QuizCategory> QuizCategories { get; set; } = [];
}