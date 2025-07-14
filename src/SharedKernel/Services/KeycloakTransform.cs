using System;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text.Json;


namespace admin_service.Infrastructure.Services;

public class KeycloakRoleClaimsTransformer : IClaimsTransformation
{
    public Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
        var claimsIdentity = principal.Identity as ClaimsIdentity;
        if (claimsIdentity == null)
            return Task.FromResult(principal);

        var realmAccessClaim = claimsIdentity.FindFirst("realm_access");
        if (realmAccessClaim != null)
        {
            var realmAccessJson = realmAccessClaim.Value;
            var realmAccess = JsonSerializer.Deserialize<JsonElement>(realmAccessJson);
            
            if (realmAccess.TryGetProperty("roles", out var roles))
            {
                foreach (var role in roles.EnumerateArray())
                {
                    var roleName = role.GetString();
                    if (!string.IsNullOrEmpty(roleName))
                    {
                        claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, roleName));
                    }
                }
            }
        }

        var resourceAccessClaim = claimsIdentity.FindFirst("resource_access");
        if (resourceAccessClaim != null)
        {
            var resourceAccessJson = resourceAccessClaim.Value;
            var resourceAccess = JsonSerializer.Deserialize<JsonElement>(resourceAccessJson);
            
            foreach (var clientProperty in resourceAccess.EnumerateObject())
            {
                var clientId = clientProperty.Name;
                if (clientProperty.Value.TryGetProperty("roles", out var clientRoles))
                {
                    foreach (var role in clientRoles.EnumerateArray())
                    {
                        var roleName = role.GetString();
                        if (!string.IsNullOrEmpty(roleName))
                        {
                            claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, $"{clientId}:{roleName}"));
                        }
                    }
                }
            }
        }

        return Task.FromResult(principal);
    }
}
