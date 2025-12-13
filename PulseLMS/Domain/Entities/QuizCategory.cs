namespace PulseLMS.Domain.Entities;

public class QuizCategory
{
    public Guid QuizId { get; set; }
    public Quiz Quiz { get; set; } = null!;

    public Guid CategoryId { get; set; }
    public Category Category { get; set; } = null!;
}