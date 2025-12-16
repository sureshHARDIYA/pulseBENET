using System.Security.Claims;

namespace PulseLMS.Common;

public interface ICurrentUser
{
    Guid? UserId { get; }
    bool IsAuthenticated { get; }
}

public sealed class CurrentUser(IHttpContextAccessor accessor) : ICurrentUser
{
    private ClaimsPrincipal? User => accessor.HttpContext?.User;

    public bool IsAuthenticated => User?.Identity?.IsAuthenticated == true;

    public Guid? UserId
    {
        get
        {
            // Supabase uses "sub" for the user id. ASP.NET may map it to NameIdentifier.
            var sub = User?.FindFirstValue("sub") ?? User?.FindFirstValue(ClaimTypes.NameIdentifier);
            return Guid.TryParse(sub, out var id) ? id : null;
        }
    }
}