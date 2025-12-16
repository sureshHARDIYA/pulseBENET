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
            var user = accessor.HttpContext?.User;
            if (user?.Identity?.IsAuthenticated != true) return null;

            // Supabase uses "sub" for user id
            var sub = user.FindFirstValue(ClaimTypes.NameIdentifier)
                      ?? user.FindFirstValue("sub");

            return Guid.TryParse(sub, out var id) ? id : null;
        }
    }
    public string? Role => accessor.HttpContext?.User?.FindFirstValue("user_role");

}