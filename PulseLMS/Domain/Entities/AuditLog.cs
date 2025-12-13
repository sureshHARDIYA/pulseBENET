namespace PulseLMS.Domain.Entities;

public abstract class AuditLog
{
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }

    // For now Just the Username, Later we will reference the User
    public string? CreatedBy { get; set; }  
    public string? UpdatedBy { get; set; }
}