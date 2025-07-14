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

        Claim? realmAccessClaim = claimsIdentity.FindFirst("realm_access");
        if (realmAccessClaim != null)
        {
            string realmAccessJson = realmAccessClaim.Value;
            JsonElement realmAccess = JsonSerializer.Deserialize<JsonElement>(realmAccessJson);

            if (realmAccess.TryGetProperty("roles", out JsonElement roles))
            {
                foreach (JsonElement role in roles.EnumerateArray())
                {
                    string? roleName = role.GetString();
                    if (!string.IsNullOrEmpty(roleName))
                    {
                        claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, roleName));
                    }
                }
            }
        }

        Claim? resourceAccessClaim = claimsIdentity.FindFirst("resource_access");
        if (resourceAccessClaim != null)
        {
            string resourceAccessJson = resourceAccessClaim.Value;
            JsonElement resourceAccess = JsonSerializer.Deserialize<JsonElement>(resourceAccessJson);

            foreach (JsonProperty clientProperty in resourceAccess.EnumerateObject())
            {
                string clientId = clientProperty.Name;
                if (clientProperty.Value.TryGetProperty("roles", out JsonElement clientRoles))
                {
                    foreach (JsonElement role in clientRoles.EnumerateArray())
                    {
                        string? roleName = role.GetString();
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
