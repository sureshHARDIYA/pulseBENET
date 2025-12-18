namespace PulseLMS.Common;

public abstract class AuditLogResponse
{
	public DateTimeOffset CreatedAt { get; init; }
	public DateTimeOffset UpdatedAt { get; init; }
	public Guid? CreatedBy { get; init; }
	public Guid? UpdatedBy { get; init; }
}