using System.Text.Json.Serialization;
using Refit;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Admin.InviteAdmin;
using Application.Abstractions.Models;
using System.Net.Http;

namespace Application.Interfaces;

public interface IKeycloakApi
{
    [Post("/realms/{realm}/protocol/openid-connect/token")]
    Task<TokenResponseDto> GetTokenAsync(
        [AliasAs("realm")] string realm,
        [Body(BodySerializationMethod.UrlEncoded)] Dictionary<string, string> parameters
    );

    [Post("/admin/realms/{realm}/organizations/{organizationId}/members/invite-user")]
    Task<Refit.ApiResponse<HttpResponseMessage>> InviteUserAsync(
    [AliasAs("realm")] string realm,
    [AliasAs("organizationId")] string organizationId,
    [Body(BodySerializationMethod.UrlEncoded)] Dictionary<string, string> requestData,
    [Header("Authorization")] string authorization
);


    [Post("/admin/realms/{realm}/organizations")]
    Task<Refit.ApiResponse<HttpResponseMessage>> CreateOrganization(
        [AliasAs("realm")] string realm,
        [Body] CreateOrganizationRequest request,
        [Header("Authorization")] string authorization);

}

public class TokenResponseDto
{
    [JsonPropertyName("access_token")]
    public string AccessToken { get; set; } = string.Empty;
}

public class ApiResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
}


public class InviteUserRequestDto
{
    public required string Email { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public Dictionary<string, List<string>> Attributes { get; set; }
}

public class CreateOrganizationRequest
{
    public required string Name { get; set; }
    public required string Alias { get; set; }
    public string RedirectUrl { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<Links> Domains { get; set; }
    public Dictionary<string, string> Attributes { get; set; } = new()
    {
        // { "type", "school" }
    };
}

public class Links
{
    public required string Name { get; set; }
    public bool Verified { get; set; }
}
