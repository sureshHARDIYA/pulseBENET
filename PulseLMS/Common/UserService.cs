using System.Security.Claims;

public interface ICurrentUser
{
    string? UserId { get; }
}

public sealed class CurrentUser(IHttpContextAccessor accessor) : ICurrentUser
{
    public string? UserId =>
        accessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier)
        ?? accessor.HttpContext?.User?.FindFirstValue("sub"); 
}