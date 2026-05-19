using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Authorization;

internal sealed class PermissionAuthorizationHandler(ILogger<PermissionAuthorizationHandler> logger) : AuthorizationHandler<PermissionRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
    {
        if (context.User?.Identity?.IsAuthenticated != true)
        {
            return Task.CompletedTask;
        }

        List<string> roles = new();

        System.Security.Claims.Claim? realmAccessClaim = context.User.Claims.FirstOrDefault(c => c.Type == "realm_access");
        if (realmAccessClaim != null)
        {
            try
            {
                Dictionary<string, JsonElement>? realmAccess = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(realmAccessClaim.Value);
                if (realmAccess != null && realmAccess.TryGetValue("roles", out JsonElement rolesElement) && rolesElement.ValueKind == JsonValueKind.Array)
                {
                    roles.AddRange(rolesElement.EnumerateArray().Select(e => e.GetString()).Where(s => s != null)!);
                }
            }
            catch (JsonException ex)
            {
                logger.LogWarning(ex, "Failed to deserialize realm_access claim");
            }
        }

        System.Security.Claims.Claim? resourceAccessClaim = context.User.Claims.FirstOrDefault(c => c.Type == "resource_access");
        if (resourceAccessClaim != null)
        {
            try
            {
                Dictionary<string, Dictionary<string, JsonElement>>? resourceAccess = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, JsonElement>>>(resourceAccessClaim.Value);
                if (resourceAccess != null)
                {
                    foreach (KeyValuePair<string, Dictionary<string, JsonElement>> client in resourceAccess)
                    {
                        if (client.Value.TryGetValue("roles", out JsonElement rolesElement) && rolesElement.ValueKind == JsonValueKind.Array)
                        {
                            roles.AddRange(rolesElement.EnumerateArray().Select(e => e.GetString()).Where(s => s != null)!);
                        }
                    }
                }
            }
            catch (JsonException ex)
            {
                logger.LogWarning(ex, "Failed to deserialize resource_access claim");
            }
        }

        IEnumerable<string> groupClaims = context.User.Claims.Where(c => c.Type == "groups").Select(c => c.Value);
        roles.AddRange(groupClaims);

        if (roles.Contains(requirement.Permission))
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}
