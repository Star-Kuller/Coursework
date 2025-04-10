using System.Security.Claims;

namespace Coursework.Extensions;

public static class UserExtensions
{
    public static long GetId(this ClaimsPrincipal user)
    {
        var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier);

        if (userIdClaim == null)
        {
            return default;
        }
        
        return long.TryParse(userIdClaim.Value, out var id) ? id : default;
    }
}