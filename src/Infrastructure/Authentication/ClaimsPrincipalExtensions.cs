using System.Diagnostics;
using System.Security.Claims;

namespace Infrastructure.Authentication;

internal static class ClaimsPrincipalExtensions
{
    public static Guid? GetUserPublicId(this ClaimsPrincipal? principal)
    {
        string? userId = principal?.FindFirst("app_user_id")?.Value;

        return Guid.TryParse(userId, out Guid parsedUserId)
            ? parsedUserId
            : null;
    }


    public static string GetUserRole(this ClaimsPrincipal? principal)
    {
        string? role = principal?.FindFirst("app_user_role")?.Value;

        return role ?? "unknow";
    }
    public static string GetUserKeycloakId(this ClaimsPrincipal? principal)
    {
        if (principal == null)
        {
            return "unknown";
        }

        string? sub = principal.FindFirst("sub")?.Value;
        if (string.IsNullOrEmpty(sub))
        {
            sub = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value ??
                  principal.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;
        }
        foreach (Claim claim in principal.Claims)
        {
            Debug.WriteLine($"Claim Type: {claim.Type}, Value: {claim.Value}");
        }

        return sub ?? "unknown";
    }
}
