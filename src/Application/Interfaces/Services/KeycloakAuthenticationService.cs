// using System;
// using System.Collections.Generic;
// using System.IdentityModel.Tokens.Jwt;
// using System.Linq;
// using System.Net.Http.Json;
// using System.Text.Json;
// using System.Text.Json.Serialization;
// using System.Threading.Tasks;
// using admin_service.Application.Common.Interfaces;
// using Microsoft.Extensions.Configuration;
// using Microsoft.Extensions.Logging;

// namespace admin_service.Infrastructure.Services
// {
//     public class KeycloakAuthenticationService : IAuthenticationService
//     {
//         private readonly HttpClient _httpClient;
//         private readonly ILogger<KeycloakAuthenticationService> _logger;
//         private readonly IConfiguration _configuration;

//         public KeycloakAuthenticationService(
//             HttpClient httpClient,
//             ILogger<KeycloakAuthenticationService> logger,
//             IConfiguration configuration)
//         {
//             _httpClient = httpClient;
//             _logger = logger;
//             _configuration = configuration;
//         }

//         public async Task<AuthResult> ValidateCredentialsAsync(string username, string password)
//         {
//             try
//             {
//                 // Ensure configuration values are not null
//                 var clientId = _configuration["Keycloak:ClientId"] 
//                     ?? throw new InvalidOperationException("Missing configuration for Keycloak:ClientId.");
//                 var clientSecret = _configuration["Keycloak:ClientSecret"] 
//                     ?? throw new InvalidOperationException("Missing configuration for Keycloak:ClientSecret.");
//                 var authority = _configuration["Keycloak:Authority"] 
//                     ?? throw new InvalidOperationException("Missing configuration for Keycloak:Authority.");

//                 var tokenRequest = new Dictionary<string, string>
//                 {
//                     ["client_id"] = clientId,
//                     ["client_secret"] = clientSecret,
//                     ["grant_type"] = "password",
//                     ["username"] = username,
//                     ["password"] = password,
//                     ["scope"] = "openid profile email"
//                 };

//                 var tokenEndpoint = $"{authority}/protocol/openid-connect/token";
//                 var response = await _httpClient.PostAsync(tokenEndpoint, new FormUrlEncodedContent(tokenRequest));

//                 if (response.IsSuccessStatusCode)
//                 {
//                     var tokenResponse = await response.Content.ReadFromJsonAsync<TokenResponse>();
//                     if (tokenResponse == null || string.IsNullOrEmpty(tokenResponse.AccessToken))
//                     {
//                         throw new Exception("Failed to parse token response.");
//                     }

//                     var handler = new JwtSecurityTokenHandler();
//                     var jwtToken = handler.ReadJwtToken(tokenResponse.AccessToken);

//                     // Extract roles from token if they exist in resource_access claim
//                     var roles = new List<string>();
//                     var resourceAccessClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "resource_access");
//                     if (resourceAccessClaim != null)
//                     {
//                         var resourceAccess = JsonSerializer.Deserialize<JsonElement>(resourceAccessClaim.Value);
//                         if (resourceAccess.TryGetProperty(clientId, out var clientAccess) &&
//                             clientAccess.TryGetProperty("roles", out var rolesElement))
//                         {
//                             foreach (var role in rolesElement.EnumerateArray())
//                             {
//                                 var roleValue = role.GetString();
//                                 if (!string.IsNullOrEmpty(roleValue))
//                                 {
//                                     roles.Add(roleValue);
//                                 }
//                             }
//                         }
//                     }

//                     // Also check for realm_access roles
//                     var realmRolesClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "realm_access");
//                     if (realmRolesClaim != null)
//                     {
//                         var realmAccess = JsonSerializer.Deserialize<JsonElement>(realmRolesClaim.Value);
//                         if (realmAccess.TryGetProperty("roles", out var rolesElement))
//                         {
//                             foreach (var role in rolesElement.EnumerateArray())
//                             {
//                                 var roleValue = role.GetString();
//                                 if (!string.IsNullOrEmpty(roleValue))
//                                 {
//                                     roles.Add(roleValue);
//                                 }
//                             }
//                         }
//                     }

//                     return new AuthResult
//                     {
//                         Succeeded = true,
//                         AccessToken = tokenResponse.AccessToken,
//                         RefreshToken = tokenResponse.RefreshToken,
//                         ExpiresAt = DateTime.UtcNow.AddSeconds(tokenResponse.ExpiresIn),
//                         Roles = roles
//                     };
//                 }

//                 _logger.LogWarning("Failed to authenticate user {Username}", username);
//                 return new AuthResult
//                 {
//                     Succeeded = false,
//                     Error = "Invalid username or password"
//                 };
//             }
//             catch (Exception ex)
//             {
//                 _logger.LogError(ex, "Error during authentication");
//                 return new AuthResult
//                 {
//                     Succeeded = false,
//                     Error = "An error occurred during authentication"
//                 };
//             }
//         }

//         public async Task<AuthResult> RefreshTokenAsync(string refreshToken)
//         {
//             try
//             {
//                 var clientId = _configuration["Keycloak:ClientId"] 
//                     ?? throw new InvalidOperationException("Missing configuration for Keycloak:ClientId.");
//                 var clientSecret = _configuration["Keycloak:ClientSecret"] 
//                     ?? throw new InvalidOperationException("Missing configuration for Keycloak:ClientSecret.");
//                 var authority = _configuration["Keycloak:Authority"] 
//                     ?? throw new InvalidOperationException("Missing configuration for Keycloak:Authority.");

//                 var tokenRequest = new Dictionary<string, string>
//                 {
//                     ["client_id"] = clientId,
//                     ["client_secret"] = clientSecret,
//                     ["grant_type"] = "refresh_token",
//                     ["refresh_token"] = refreshToken
//                 };

//                 var tokenEndpoint = $"{authority}/protocol/openid-connect/token";
//                 var response = await _httpClient.PostAsync(tokenEndpoint, new FormUrlEncodedContent(tokenRequest));

//                 if (response.IsSuccessStatusCode)
//                 {
//                     var tokenResponse = await response.Content.ReadFromJsonAsync<TokenResponse>();
//                     if (tokenResponse == null || string.IsNullOrEmpty(tokenResponse.AccessToken))
//                     {
//                         throw new Exception("Failed to parse token response.");
//                     }

//                     return new AuthResult
//                     {
//                         Succeeded = true,
//                         AccessToken = tokenResponse.AccessToken,
//                         RefreshToken = tokenResponse.RefreshToken,
//                         ExpiresAt = DateTime.UtcNow.AddSeconds(tokenResponse.ExpiresIn)
//                     };
//                 }

//                 return new AuthResult
//                 {
//                     Succeeded = false,
//                     Error = "Invalid or expired refresh token"
//                 };
//             }
//             catch (Exception ex)
//             {
//                 _logger.LogError(ex, "Error during token refresh");
//                 return new AuthResult
//                 {
//                     Succeeded = false,
//                     Error = "An error occurred during token refresh"
//                 };
//             }
//         }

//         public async Task RevokeTokenAsync(string refreshToken)
//         {
//             try
//             {
//                 var clientId = _configuration["Keycloak:ClientId"] 
//                     ?? throw new InvalidOperationException("Missing configuration for Keycloak:ClientId.");
//                 var clientSecret = _configuration["Keycloak:ClientSecret"] 
//                     ?? throw new InvalidOperationException("Missing configuration for Keycloak:ClientSecret.");
//                 var authority = _configuration["Keycloak:Authority"] 
//                     ?? throw new InvalidOperationException("Missing configuration for Keycloak:Authority.");

//                 var logoutRequest = new Dictionary<string, string>
//                 {
//                     ["client_id"] = clientId,
//                     ["client_secret"] = clientSecret,
//                     ["refresh_token"] = refreshToken
//                 };

//                 var logoutEndpoint = $"{authority}/protocol/openid-connect/logout";
//                 await _httpClient.PostAsync(logoutEndpoint, new FormUrlEncodedContent(logoutRequest));
//             }
//             catch (Exception ex)
//             {
//                 _logger.LogError(ex, "Error during token revocation");
//             }
//         }

//         private class TokenResponse
//         {
//             [JsonPropertyName("access_token")]
//             public string? AccessToken { get; set; }
            
//             [JsonPropertyName("expires_in")]
//             public int ExpiresIn { get; set; }
            
//             [JsonPropertyName("refresh_token")]
//             public string? RefreshToken { get; set; }
            
//             [JsonPropertyName("token_type")]
//             public string? TokenType { get; set; }
//         }
//     }
// }
// using System;
// using System.Collections.Generic;
// using System.Net.Http;
// using System.Net.Http.Headers;
// using System.Net.Http.Json;
// using System.Text;
// using System.Text.Json;
// using System.Threading.Tasks;
// using admin_service.Application.Common.Interfaces;
// using admin_service.Infrastructure.Repositories;
// using Keycloak.Net;
// using Keycloak.Net.Models.Root;
// using Keycloak.Net.Models.Users;
// using Microsoft.Extensions.Configuration;

// public class KeycloakService(IConfiguration configuration) : IKeycloakService
// {
//     private readonly IConfiguration _configuration = configuration;
//     private readonly string? _baseUrl;
//     private readonly string? _realm;
//     private readonly string? _clientId;
//     private readonly string? _clientSecret;
//     private readonly string? _adminUsername;
//     private readonly string? _adminPassword;

//     public async Task<string> GetAdminAccessTokenAsync()
//     {
//         using var client = new HttpClient();
//         var request = new Dictionary<string, string>
//         {
//             { "grant_type", "password" },
//             { "client_id", _clientId },
//             { "client_secret", _clientSecret },
//             { "username", _adminUsername },
//             { "password", _adminPassword }
//         };

//         var response = await client.PostAsync($"{_baseUrl}/realms/{_realm}/protocol/openid-connect/token",
//             new FormUrlEncodedContent(request));

//         if (!response.IsSuccessStatusCode)
//             throw new Exception("Failed to retrieve access token");

//         var jsonResponse = await response.Content.ReadAsStringAsync();
//         using var doc = JsonDocument.Parse(jsonResponse);
//         var accessToken = doc.RootElement.GetProperty("access_token").GetString();
//         if (accessToken == null)
//         {
//             throw new Exception("The access token was not present in the response.");
//         }
//         return accessToken;

//     }

//     public async Task<bool> CreateUserAsync(User user)
//     {
//         var token = await GetAdminAccessTokenAsync();
//         var keycloakClient = new KeycloakClient(_baseUrl, _realm, token);
//         return await keycloakClient.CreateUserAsync(_realm, user);
//     }

//     public async Task<bool> SendResetPasswordEmailAsync(string userId)
//     {
//         var token = await GetAdminAccessTokenAsync();
//         using var client = new HttpClient();
//         client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

//         var response = await client.PutAsync($"{_baseUrl}/admin/realms/{_realm}/users/{userId}/execute-actions-email",
//             new StringContent(JsonSerializer.Serialize(new[] { "UPDATE_PASSWORD" }), Encoding.UTF8, "application/json"));

//         return response.IsSuccessStatusCode;
//     }
// }

// public class KeycloakService : IKeycloakService
// {
//     private readonly string _baseUrl;
//     private readonly string _realm;
//     private readonly string _clientId;
//     private readonly string _clientSecret;
//     private readonly string _adminUsername;
//     private readonly string _adminPassword;

//     public KeycloakService(IConfiguration configuration)
//     {
//         _baseUrl = configuration["Keycloak:BaseUrl"]!;
//         _realm = configuration["Keycloak:Realm"]!;
//         _clientId = configuration["Keycloak:ClientId"]!;
//         _clientSecret = configuration["Keycloak:ClientSecret"]!;
//         _adminUsername = configuration["Keycloak:AdminUsername"]!;
//         _adminPassword = configuration["Keycloak:AdminPassword"]!;
//     }

//     public async Task<string> GetAdminAccessTokenAsync()
//     {
//         using var client = new HttpClient();
//         var request = new Dictionary<string, string>
//         {
//             { "grant_type", "password" },
//             { "client_id", _clientId },
//             { "client_secret", _clientSecret },
//             { "username", _adminUsername },
//             { "password", _adminPassword }
//         };

//         var response = await client.PostAsync($"{_baseUrl}/realms/{_realm}/protocol/openid-connect/token",
//             new FormUrlEncodedContent(request));

//         if (!response.IsSuccessStatusCode)
//             throw new Exception("Failed to retrieve access token");

//         var jsonResponse = await response.Content.ReadAsStringAsync();
//         using var doc = JsonDocument.Parse(jsonResponse);
        
//         var accessToken = doc.RootElement.GetProperty("access_token").GetString();

//         return accessToken ?? throw new Exception("Access token not found in response.");
//     }


//     public async Task<bool> CreateUserAsync(User user)
//     {
//         var token = await GetAdminAccessTokenAsync();
//         var keycloakClient = new KeycloakClient(_baseUrl, _realm, token);
//         return await keycloakClient.CreateUserAsync(_realm, user);
//     }

//     public async Task<bool> SendResetPasswordEmailAsync(string userId)
//     {
//         var token = await GetAdminAccessTokenAsync();
//         using var client = new HttpClient();
//         client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

//         var response = await client.PutAsync($"{_baseUrl}/admin/realms/{_realm}/users/{userId}/execute-actions-email",
//             new StringContent(JsonSerializer.Serialize(new[] { "UPDATE_PASSWORD" }), Encoding.UTF8, "application/json"));

//         return response.IsSuccessStatusCode;
//     }
// }
// public class CreateUserDto
// {
//     public string FirstName { get; set; } = string.Empty;
//     public string LastName { get; set; } = string.Empty;
//     public string Email { get; set; } = string.Empty;
//     public string Password { get; set; } = string.Empty; 
// }


    // public class KeycloakService : admin_service.Application.Common.Interfaces.IKeycloakService
    // {
    //     private readonly HttpClient _httpClient;
    //     private readonly IConfiguration _configuration;

    //     public KeycloakService(HttpClient httpClient, IConfiguration configuration)
    //     {
    //         _httpClient = httpClient;
    //         _configuration = configuration;
    //     }

    //     // public async Task<bool> CreateUserAsync(InviteUserRequest request)
    //     // {
            
    //     // }

    // public async Task<bool> CreateUserAsync(admin_service.Application.Common.Interfaces.InviteUserRequest request)
    // {
    //     var adminToken = await GetAdminTokenAsync();
    //         _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", adminToken);

    //         var keycloakUser = new
    //         {
    //             username = request.Email,
    //             email = request.Email,
    //             enabled = true,
    //             firstName = request.FirstName,
    //             lastName = request.LastName
    //         };

    //         var keycloakUrl = $"{_configuration["Keycloak:BaseUrl"]}/admin/realms/{_configuration["Keycloak:Realm"]}/users";
    //         var response = await _httpClient.PostAsJsonAsync(keycloakUrl, keycloakUser);
    //         return response.IsSuccessStatusCode;
    // }

    // private async Task<string?> GetAdminTokenAsync()
    //     {
    //         var tokenEndpoint = $"{_configuration["Keycloak:BaseUrl"]}/realms/{_configuration["Keycloak:Realm"]}/protocol/openid-connect/token";
    //         var clientId = _configuration["Keycloak:ClientId"]!;
    //         var clientSecret = _configuration["Keycloak:ClientSecret"]!;

    //         var parameters = new Dictionary<string, string>
    //         {
    //             { "client_id", clientId },
    //             { "client_secret", clientSecret },
    //             { "grant_type", "client_credentials" }
    //         };

    //         var response = await _httpClient.PostAsync(tokenEndpoint, new FormUrlEncodedContent(parameters));
    //         if(response.IsSuccessStatusCode)
    //         {
    //             var json = await response.Content.ReadAsStringAsync();
    //             var tokenResponse = JsonSerializer.Deserialize<TokenResponse>(json);
    //             return tokenResponse?.AccessToken;
    //         }
    //         throw new Exception("Unable to retrieve Keycloak admin token.");
    //     }
    // }

    // //     public class InviteUserRequest
    // // {
    // //     public string? Email { get; set; }
    // //     public string? FirstName { get; set; }
    // //     public string? LastName { get; set; }
    // // }

    // // public class TokenResponse
    // // {
    // //     public string? AccessToken { get; set; }
    // // }

using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System;
using Refit;


public class KeycloakService
{
    private readonly IKeycloakApi _keycloakApi;
    private readonly IConfiguration _configuration;

    public KeycloakService(IKeycloakApi keycloakApi, IConfiguration configuration)
    {
        _keycloakApi = keycloakApi;
        _configuration = configuration;
    }

    public async Task<string> GetAdminAccessTokenAsync()
    {
        var parameters = new Dictionary<string, string>
        {
            { "grant_type", "client_credentials" },
            { "client_id", _configuration["Keycloak:ClientId"]! },
            { "client_secret", _configuration["Keycloak:ClientSecret"]! }
        };

        var response = await _keycloakApi.GetTokenAsync(_configuration["Keycloak:Realm"]!, parameters);
        return response.AccessToken;
    }

public async Task<ApiResponse<HttpResponseMessage>> InviteUserAsync(InviteUserRequestDto request)
{
    var requestData = new Dictionary<string, string>
    {
        { "email", request.Email },
        { "firstName", request.FirstName },
        { "lastName", request.LastName }
    };

    var token = await GetAdminAccessTokenAsync();
    return await _keycloakApi.InviteUserAsync(_configuration["Keycloak:Realm"]!, _configuration["Keycloak:organizationId"]!, requestData, $"Bearer {token}");
}


     public async Task<string> CreateOrganizationAsync(string schoolName)
{
    var token = await GetAdminAccessTokenAsync();
    var realm = _configuration["Keycloak:Realm"]!;
    
    var request = new CreateOrganizationRequest
    {
        Name = schoolName.Replace(" ", ""),
        Alias = schoolName.Replace(" ", ""),
        Description = "",
        RedirectUrl = "",
        Domains = new List<Domain> { new Domain { Name = $"{schoolName.Replace(" ", "")}.com" } }
    };

    var response = await _keycloakApi.CreateOrganization(realm, request, $"Bearer {token}");

     if (!response.IsSuccessStatusCode)
    {
        throw new Exception($"Failed to create organization");
    }

    // Extract organization ID from the "Location" header if it exists
    if (response.Headers.TryGetValues("Location", out var locationValues))
    {
        var locationUrl = locationValues.FirstOrDefault();
        if (locationUrl != null)
        {
            var orgId = locationUrl.Split('/').Last();
            Console.WriteLine($"cxcghfghc {orgId}");
            return orgId;
        }
    }

    throw new Exception("Organization created, but no ID found in response.");

}

}
