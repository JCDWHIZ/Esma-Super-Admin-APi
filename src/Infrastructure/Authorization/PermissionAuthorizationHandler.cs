using System.Text.Json;
using Infrastructure.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Authorization;

internal sealed class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
    {
        if (context.User?.Identity?.IsAuthenticated != true)
        {
            return Task.CompletedTask;
        }

        var roles = new List<string>();

        // 1. Extract roles from realm_access
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
                Console.WriteLine($"Failed to deserialize realm_access claim: {ex.Message}"); // Replace with your logger
            }
        }

        // 2. Extract roles from resource_access
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
                Console.WriteLine($"Failed to deserialize resource_access claim: {ex.Message}"); // Replace with your logger
            }
        }

        // 3. Extract roles from groups claims
        IEnumerable<string> groupClaims = context.User.Claims.Where(c => c.Type == "groups").Select(c => c.Value);
        roles.AddRange(groupClaims);

        // Debugging: Log extracted roles
        Console.WriteLine($"Extracted roles: {string.Join(", ", roles)}"); // Replace with your logger

        // Check if the required permission is in the roles
        if (roles.Contains(requirement.Permission))
        {
            context.Succeed(requirement);
        }
        else
        {
            Console.WriteLine($"Permission '{requirement.Permission}' not found in roles."); // Replace with your logger
        }

        return Task.CompletedTask;
    }
}
