using System.Security.Claims;
using System.Security.Principal;

namespace GlowingStoreApplication.Authentication.Extensions;

public static class ClaimsExtensions
{
    public static Guid GetId(this IPrincipal user)
    {
        var value = GetClaimValue(user, ClaimTypes.NameIdentifier);
        if (Guid.TryParse(value, out var userId))
        {
            return userId;
        }

        return Guid.Empty;
    }

    public static string GetUserName(this IPrincipal user)
        => GetClaimValue(user, ClaimTypes.Name);

    public static string GetEmail(this IPrincipal user)
        => GetClaimValue(user, ClaimTypes.Email);

    public static string GetClaimValue(this IPrincipal user, string claimType)
    {
        var value = ((ClaimsPrincipal)user).FindFirst(claimType)?.Value;
        return value;
    }
}