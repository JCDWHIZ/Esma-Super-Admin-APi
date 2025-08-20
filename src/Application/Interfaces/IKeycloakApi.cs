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


    [Post("/admin/realms/{realm}/users")]
    Task<Refit.ApiResponse<HttpResponseMessage>> CreateUserAsync(
        [AliasAs("realm")] string realm,
        [Body] InviteUserRequestDto request,
        [Header("Authorization")] string authorization);

    [Put("/admin/realms/{realm}/users/{id}")]
    Task<Refit.ApiResponse<HttpResponseMessage>> UpdateUserAsync(
    [AliasAs("realm")] string realm,
    [AliasAs("id")] string userId,
    [Body] UpdateUserRequestDto request,
    [Header("Authorization")] string authorization);

    [Delete("/admin/realms/{realm}/users/{id}")]
    Task<Refit.ApiResponse<HttpResponseMessage>> DeleteUserAsync(
        [AliasAs("realm")] string realm,
        [AliasAs("id")] string userId,
        [Header("Authorization")] string authorization);

    // [Put("/admin/realms/{realm}/organizations/{organizationId}/members/{userId}")]
    // Task<Refit.ApiResponse<HttpResponseMessage>> AddUserToOrganizationAsync(
    //     [AliasAs("realm")] string realm,
    //     [AliasAs("organizationId")] string organizationId,
    //     [AliasAs("userId")] string userId,
    //     [Header("Authorization")] string authorization);

    [Post("/admin/realms/{realm}/organizations/{organizationId}/members")]
    [Headers("Content-Type: application/json")]
    Task AddUserToOrganizationAsync(
      [AliasAs("realm")] string realm,
      [AliasAs("organizationId")] string organizationId,
      [Body] string userId,
      [Header("Authorization")] string authorization);


    [Post("/realms/{realm}/protocol/openid-connect/token")]
    Task<LoginResponseDto> LoginAsync(
      [AliasAs("realm")] string realm,
      [Body(BodySerializationMethod.UrlEncoded)] Dictionary<string, string> parameters
  );

    [Post("/realms/{realm}/protocol/openid-connect/logout")]
    Task<Refit.ApiResponse<HttpResponseMessage>> LogoutAsync(
        [AliasAs("realm")] string realm,
        [Body(BodySerializationMethod.UrlEncoded)] Dictionary<string, string> parameters
    );

    [Post("/realms/{realm}/protocol/openid-connect/token")]
    Task<RefreshTokenResponseDto> RefreshTokenAsync(
        [AliasAs("realm")] string realm,
        [Body(BodySerializationMethod.UrlEncoded)] Dictionary<string, string> parameters
    );

    [Post("/admin/realms/{realm}/users/{userId}/execute-actions-email")]
    Task<Refit.ApiResponse<HttpResponseMessage>> SendResetPasswordEmailAsync(
        [AliasAs("realm")] string realm,
        [AliasAs("userId")] string userId,
        [Body] List<string> actions,
        [Header("Authorization")] string authorization,
        [Query("client_id")] string clientId
    // [Query("redirect_uri")] string redirectUri
    );

    [Get("/admin/realms/{realm}/users")]
    Task<List<KeycloakUserDto>> GetUsersByEmailAsync(
        [AliasAs("realm")] string realm,
        [Query("email")] string email,
        [Header("Authorization")] string authorization
    );

    [Post("/realms/{realm}/protocol/openid-connect/token/introspect")]
    Task<TokenIntrospectionResponseDto> IntrospectTokenAsync(
        [AliasAs("realm")] string realm,
        [Body(BodySerializationMethod.UrlEncoded)] Dictionary<string, string> parameters
    );

    [Post("/realms/{realm}/account/credentials/password")]
    Task<Refit.ApiResponse<HttpResponseMessage>> ChangePasswordAsync(
        [AliasAs("realm")] string realm,
        [Body] ChangePasswordRequest request,
        [Header("Authorization")] string authorization
    );

    // Required Actions - for setting up new users
    [Put("/admin/realms/{realm}/users/{userId}")]
    Task<Refit.ApiResponse<HttpResponseMessage>> UpdateUserAsync(
        [AliasAs("realm")] string realm,
        [AliasAs("userId")] string userId,
        [Body] UpdateUserRequest request,
        [Header("Authorization")] string authorization
    );

    [Post("/admin/realms/{realm}/users/{userId}/execute-actions-email")]
    Task<Refit.ApiResponse<HttpResponseMessage>> SendRequiredActionsEmailAsync(
        [AliasAs("realm")] string realm,
        [AliasAs("userId")] string userId,
        [Body] List<string> actions,
        [Header("Authorization")] string authorization,
        [Query("client_id")] string clientId,
        // [Query("redirect_uri")] string redirectUri,
        [Query("lifespan")] int? lifespan = null
    );

    [Put("/admin/realms/{realm}/users/{userId}/reset-password")]
    Task ResetPasswordAsync(string realm, string userId, [Body] PasswordResetRequest request, [Header("Authorization")] string authorization);

    // Realm Roles Management
    [Get("/admin/realms/{realm}/roles")]
    Task<Refit.ApiResponse<List<KeycloakRoleDto>>> GetRealmRolesAsync(
        [AliasAs("realm")] string realm,
        [Header("Authorization")] string authorization
    );

    [Get("/admin/realms/{realm}/roles/{roleName}")]
    Task<Refit.ApiResponse<KeycloakRoleDto>> GetRealmRoleByNameAsync(
        [AliasAs("realm")] string realm,
        [AliasAs("roleName")] string roleName,
        [Header("Authorization")] string authorization
    );

    [Post("/admin/realms/{realm}/roles")]
    Task<Refit.ApiResponse<HttpResponseMessage>> CreateRealmRoleAsync(
        [AliasAs("realm")] string realm,
        [Body] CreateKeycloakRoleDto role,
        [Header("Authorization")] string authorization
    );

    [Put("/admin/realms/{realm}/roles/{roleName}")]
    Task<Refit.ApiResponse<HttpResponseMessage>> UpdateRealmRoleAsync(
        [AliasAs("realm")] string realm,
        [AliasAs("roleName")] string roleName,
        [Body] UpdateKeycloakRoleDto role,
        [Header("Authorization")] string authorization
    );

    [Delete("/admin/realms/{realm}/roles/{roleName}")]
    Task<Refit.ApiResponse<HttpResponseMessage>> DeleteRealmRoleAsync(
        [AliasAs("realm")] string realm,
        [AliasAs("roleName")] string roleName,
        [Header("Authorization")] string authorization
    );

    // Composite Roles Management
    [Get("/admin/realms/{realm}/roles/{roleName}/composites")]
    Task<Refit.ApiResponse<List<KeycloakRoleDto>>> GetCompositeRolesAsync(
        [AliasAs("realm")] string realm,
        [AliasAs("roleName")] string roleName,
        [Header("Authorization")] string authorization
    );

    [Post("/admin/realms/{realm}/roles/{roleName}/composites")]
    Task<Refit.ApiResponse<HttpResponseMessage>> AddCompositeRolesAsync(
        [AliasAs("realm")] string realm,
        [AliasAs("roleName")] string roleName,
        [Body] List<KeycloakRoleDto> compositeRoles,
        [Header("Authorization")] string authorization
    );

    [Delete("/admin/realms/{realm}/roles/{roleName}/composites")]
    Task<Refit.ApiResponse<HttpResponseMessage>> RemoveCompositeRolesAsync(
        [AliasAs("realm")] string realm,
        [AliasAs("roleName")] string roleName,
        [Body] List<KeycloakRoleDto> compositeRoles,
        [Header("Authorization")] string authorization
    );

    // Client Roles Management
    [Get("/admin/realms/{realm}/clients/{clientId}/roles")]
    Task<Refit.ApiResponse<List<KeycloakRoleDto>>> GetClientRolesAsync(
        [AliasAs("realm")] string realm,
        [AliasAs("clientId")] string clientId,
        [Header("Authorization")] string authorization
    );

    [Post("/admin/realms/{realm}/clients/{clientId}/roles")]
    Task<Refit.ApiResponse<HttpResponseMessage>> CreateClientRoleAsync(
        [AliasAs("realm")] string realm,
        [AliasAs("clientId")] string clientId,
        [Body] CreateKeycloakRoleDto role,
        [Header("Authorization")] string authorization
    );

    [Get("/admin/realms/{realm}/clients/{clientId}/roles/{roleName}")]
    Task<Refit.ApiResponse<KeycloakRoleDto>> GetClientRoleByNameAsync(
        [AliasAs("realm")] string realm,
        [AliasAs("clientId")] string clientId,
        [AliasAs("roleName")] string roleName,
        [Header("Authorization")] string authorization
    );

    // User Role Assignments
    [Get("/admin/realms/{realm}/users/{userId}/role-mappings/realm")]
    Task<Refit.ApiResponse<List<KeycloakRoleDto>>> GetUserRealmRolesAsync(
        [AliasAs("realm")] string realm,
        [AliasAs("userId")] string userId,
        [Header("Authorization")] string authorization
    );

    [Post("/admin/realms/{realm}/users/{userId}/role-mappings/realm")]
    Task<Refit.ApiResponse<HttpResponseMessage>> AssignRealmRolesToUserAsync(
        [AliasAs("realm")] string realm,
        [AliasAs("userId")] string userId,
        [Body] List<KeycloakRoleDto> roles,
        [Header("Authorization")] string authorization
    );

    [Delete("/admin/realms/{realm}/users/{userId}/role-mappings/realm")]
    Task<Refit.ApiResponse<HttpResponseMessage>> RemoveRealmRolesFromUserAsync(
        [AliasAs("realm")] string realm,
        [AliasAs("userId")] string userId,
        [Body] List<KeycloakRoleDto> roles,
        [Header("Authorization")] string authorization
    );
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
    public required string Username { get; set; }
    public required string Email { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }

    public bool Enabled { get; set; } = true;

    public Dictionary<string, List<string>> Attributes { get; set; } = new();

    [JsonPropertyName("emailVerified")]
    public bool EmailVerified { get; set; } = true;
    [JsonPropertyName("requiredActions")]
    public List<string> RequiredActions { get; set; } = new();
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

// New Auth-related DTOs
public class LoginResponseDto
{
    [JsonPropertyName("access_token")]
    public string AccessToken { get; set; } = string.Empty;

    [JsonPropertyName("expires_in")]
    public int ExpiresIn { get; set; }

    [JsonPropertyName("refresh_expires_in")]
    public int RefreshExpiresIn { get; set; }

    [JsonPropertyName("refresh_token")]
    public string RefreshToken { get; set; } = string.Empty;

    [JsonPropertyName("token_type")]
    public string TokenType { get; set; } = string.Empty;

    [JsonPropertyName("id_token")]
    public string IdToken { get; set; } = string.Empty;

    [JsonPropertyName("not-before-policy")]
    public int NotBeforePolicy { get; set; }

    [JsonPropertyName("session_state")]
    public string SessionState { get; set; } = string.Empty;

    [JsonPropertyName("scope")]
    public string Scope { get; set; } = string.Empty;
}

public class RefreshTokenResponseDto
{
    [JsonPropertyName("access_token")]
    public string AccessToken { get; set; } = string.Empty;

    [JsonPropertyName("expires_in")]
    public int ExpiresIn { get; set; }

    [JsonPropertyName("refresh_expires_in")]
    public int RefreshExpiresIn { get; set; }

    [JsonPropertyName("refresh_token")]
    public string RefreshToken { get; set; } = string.Empty;

    [JsonPropertyName("token_type")]
    public string TokenType { get; set; } = string.Empty;

    [JsonPropertyName("id_token")]
    public string IdToken { get; set; } = string.Empty;

    [JsonPropertyName("not-before-policy")]
    public int NotBeforePolicy { get; set; }

    [JsonPropertyName("session_state")]
    public string SessionState { get; set; } = string.Empty;

    [JsonPropertyName("scope")]
    public string Scope { get; set; } = string.Empty;
}

public class TokenIntrospectionResponseDto
{
    [JsonPropertyName("active")]
    public bool Active { get; set; }

    [JsonPropertyName("scope")]
    public string Scope { get; set; } = string.Empty;

    [JsonPropertyName("username")]
    public string Username { get; set; } = string.Empty;

    [JsonPropertyName("exp")]
    public long Exp { get; set; }

    [JsonPropertyName("iat")]
    public long Iat { get; set; }

    [JsonPropertyName("sub")]
    public string Sub { get; set; } = string.Empty;

    [JsonPropertyName("aud")]
    public string Aud { get; set; } = string.Empty;

    [JsonPropertyName("iss")]
    public string Iss { get; set; } = string.Empty;

    [JsonPropertyName("jti")]
    public string Jti { get; set; } = string.Empty;

    [JsonPropertyName("client_id")]
    public string ClientId { get; set; } = string.Empty;

    [JsonPropertyName("token_type")]
    public string TokenType { get; set; } = string.Empty;
}

public class KeycloakUserDto
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("username")]
    public string Username { get; set; } = string.Empty;

    [JsonPropertyName("email")]
    public string Email { get; set; } = string.Empty;

    [JsonPropertyName("firstName")]
    public string FirstName { get; set; } = string.Empty;

    [JsonPropertyName("lastName")]
    public string LastName { get; set; } = string.Empty;

    [JsonPropertyName("enabled")]
    public bool Enabled { get; set; }

    [JsonPropertyName("emailVerified")]
    public bool EmailVerified { get; set; }

    [JsonPropertyName("createdTimestamp")]
    public long CreatedTimestamp { get; set; }
}

public class ChangePasswordRequest
{
    [JsonPropertyName("currentPassword")]
    public required string CurrentPassword { get; set; }

    [JsonPropertyName("newPassword")]
    public required string NewPassword { get; set; }

    [JsonPropertyName("confirmation")]
    public required string Confirmation { get; set; }
}

public class SetPasswordResponseDto
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
}

public class UpdateUserRequest
{
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("username")]
    public string? Username { get; set; }

    [JsonPropertyName("email")]
    public string? Email { get; set; }

    [JsonPropertyName("firstName")]
    public string? FirstName { get; set; }

    [JsonPropertyName("lastName")]
    public string? LastName { get; set; }

    [JsonPropertyName("enabled")]
    public bool Enabled { get; set; } = true;

    [JsonPropertyName("emailVerified")]
    public bool EmailVerified { get; set; } = false;

    [JsonPropertyName("requiredActions")]
    public List<string> RequiredActions { get; set; } = new();

    [JsonPropertyName("attributes")]
    public Dictionary<string, List<string>> Attributes { get; set; } = new();
}
public class PasswordResetRequest
{
    [JsonPropertyName("temporary")]
    public bool Temporary { get; set; }
    [JsonPropertyName("type")]
    public string Type { get; set; }
    [JsonPropertyName("value")]
    public string Value { get; set; }
}

public class KeycloakRoleDto
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool Composite { get; set; }
    public bool ClientRole { get; set; }
    public string ContainerId { get; set; } = string.Empty;
    public Dictionary<string, List<string>>? Attributes { get; set; }
}

public class CreateKeycloakRoleDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool Composite { get; set; }
    public Dictionary<string, List<string>>? Attributes { get; set; }
}

public class UpdateKeycloakRoleDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool Composite { get; set; }
    public Dictionary<string, List<string>>? Attributes { get; set; }
}

public class UpdateUserRequestDto
{
    public string? Username { get; set; }
    public string? Email { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public bool? Enabled { get; set; }
    public Dictionary<string, List<string>>? Attributes { get; set; }
}
