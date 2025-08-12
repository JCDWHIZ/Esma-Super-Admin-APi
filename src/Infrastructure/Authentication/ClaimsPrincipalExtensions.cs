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
}
