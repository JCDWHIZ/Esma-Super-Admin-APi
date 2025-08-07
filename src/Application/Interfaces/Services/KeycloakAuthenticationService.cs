// using System;
// using System.Collections.Generic;
// using System.IdentityModel.Tokens.Jwt;
// using System.Linq;
// using System.Net.Http.Json;
// using System.Text.Json;
// using System.Text.Json.Serialization;
// using System.Threading.Tasks;
// using Application.Interfaces;
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
// using Application.Interfaces;
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


// public class KeycloakService : Application.Interfaces.IKeycloakService
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

// public async Task<bool> CreateUserAsync(Application.Interfaces.InviteUserRequest request)
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
using Application.Abstractions.Models;
using Application.Interfaces;
using Application.Auth.Login;
using Application.Auth.ForgotPassword;
using System.Security.Claims;
using Application.Abstractions.Authentication;


namespace Application.Interfaces.Services;

public class KeycloakService
{
    private readonly IKeycloakApi _keycloakApi;
    private readonly IConfiguration _configuration;
    private readonly IApplicationDbContext _dbContext;
    private readonly IEmailService _emailService;
    private readonly ITokenProvider _tokenService;

    public KeycloakService(IKeycloakApi keycloakApi, IConfiguration configuration, IApplicationDbContext dbContext, IEmailService emailService, ITokenProvider tokenService)
    {
        _keycloakApi = keycloakApi;
        _configuration = configuration;
        _emailService = emailService;
        _tokenService = tokenService;
        _dbContext = dbContext;
    }

    public async Task<string> GetAdminAccessTokenAsync()
    {
        var parameters = new Dictionary<string, string>
        {
            { "grant_type", "client_credentials" },
            { "client_id", _configuration["Keycloak:ClientId"]! },
            { "client_secret", _configuration["Keycloak:ClientSecret"]! }
        };

        TokenResponseDto response = await _keycloakApi.GetTokenAsync(_configuration["Keycloak:Realm"]!, parameters);
        return response.AccessToken;
    }

    public async Task<Refit.ApiResponse<HttpResponseMessage>> InviteUserAsync(InviteUserRequestDto request)
    {
        var requestData = new Dictionary<string, string>
    {
        { "email", request.Email },
        { "firstName", request.FirstName },
        { "lastName", request.LastName }
    };

        string token = await GetAdminAccessTokenAsync();
        return await _keycloakApi.InviteUserAsync(_configuration["Keycloak:Realm"]!, _configuration["Keycloak:organizationId"]!, requestData, $"Bearer {token}");
    }


    public async Task<string> CreateOrganizationAsync(string schoolName)
    {
        string token = await GetAdminAccessTokenAsync();
        string realm = _configuration["Keycloak:Realm"]!;

        var request = new CreateOrganizationRequest
        {
            Name = schoolName.Replace(" ", ""),
            Alias = schoolName.Replace(" ", ""),
            Description = "",
            RedirectUrl = "",
            Domains = [new() { Name = $"{schoolName.Replace(" ", "")}.com" }]
        };

        Refit.ApiResponse<HttpResponseMessage> response = await _keycloakApi.CreateOrganization(realm, request, $"Bearer {token}");

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"Failed to create organization");
        }

        // Extract organization ID from the "Location" header if it exists
        if (response.Headers.TryGetValues("Location", out IEnumerable<string>? locationValues))
        {
            string? locationUrl = locationValues.FirstOrDefault();
            if (locationUrl != null)
            {
                string orgId = locationUrl.Split('/')[^1];
                Console.WriteLine($"OrgId {orgId}");
                return orgId;
            }
        }

        throw new Exception("Organization created, but no ID found in response.");

    }

    public async Task<string> CreateUserAsync(InviteUserRequestDto request)
    {
        string token = await GetAdminAccessTokenAsync();
        Refit.ApiResponse<HttpResponseMessage> response = await _keycloakApi.CreateUserAsync(_configuration["Keycloak:Realm"]!, request, $"Bearer {token}");

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception("Failed to create Keycloak user");
        }

        if (response.Headers.TryGetValues("Location", out IEnumerable<string>? values))
        {
            string? location = values.FirstOrDefault();
            return location?.Split('/')[^1] ?? throw new Exception("User ID missing in Keycloak response");
        }

        throw new Exception("User creation succeeded but no ID returned");
    }

    public async Task AddUserToOrganizationAsync(string userId)
    {
        string token = await GetAdminAccessTokenAsync();

        await _keycloakApi.AddUserToOrganizationAsync(
            _configuration["Keycloak:Realm"]!,
            _configuration["Keycloak:organizationId"]!,
            userId,
            $"Bearer {token}"
        );
    }

    public async Task<LoginCommandResponseDto> LoginAsync(string email, string password)
    {
        var parameters = new Dictionary<string, string>
        {
            { "grant_type", "password" },
            { "client_id", _configuration["Keycloak:ClientId"]! },
            { "client_secret", _configuration["Keycloak:ClientSecret"]! },
            { "username", email },
            { "password", password },
            { "scope", "openid profile email" }
        };

        try
        {
            LoginResponseDto response = await _keycloakApi.LoginAsync(_configuration["Keycloak:Realm"]!, parameters);

            // Get user information separately since we're not parsing JWT
            KeycloakUserDto? userInfo = await GetUserByEmailAsync(email);

            return new LoginCommandResponseDto
            {
                AccessToken = response.AccessToken,
                RefreshToken = response.RefreshToken,
                IdToken = response.IdToken,
                ExpiresIn = response.ExpiresIn,
                RefreshExpiresIn = response.RefreshExpiresIn,
                TokenType = response.TokenType,
                SessionState = response.SessionState,
                Scope = response.Scope,
                UserId = userInfo?.Id ?? string.Empty,
                Username = userInfo?.Username ?? string.Empty,
                Email = userInfo?.Email ?? email,
                ExpiresAt = DateTime.UtcNow.AddSeconds(response.ExpiresIn),
                RefreshExpiresAt = DateTime.UtcNow.AddSeconds(response.RefreshExpiresIn)
            };
        }
        catch (Refit.ApiException ex)
        {
            // Handle authentication errors
            if (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                throw new UnauthorizedAccessException("Invalid email or password");
            }
            throw new Exception($"Login failed: {ex.Message}");
        }
    }

    public async Task<RefreshTokenResponseDto> RefreshTokenAsync(string refreshToken)
    {
        var parameters = new Dictionary<string, string>
        {
            { "grant_type", "refresh_token" },
            { "client_id", _configuration["Keycloak:ClientId"]! },
            { "client_secret", _configuration["Keycloak:ClientSecret"]! },
            { "refresh_token", refreshToken }
        };

        try
        {
            return await _keycloakApi.RefreshTokenAsync(_configuration["Keycloak:Realm"]!, parameters);
        }
        catch (Refit.ApiException ex)
        {
            throw new Exception($"Token refresh failed: {ex.Message}");
        }
    }

    public async Task<bool> LogoutAsync(string refreshToken)
    {
        var parameters = new Dictionary<string, string>
        {
            { "client_id", _configuration["Keycloak:ClientId"]! },
            { "client_secret", _configuration["Keycloak:ClientSecret"]! },
            { "refresh_token", refreshToken }
        };

        try
        {
            Refit.ApiResponse<HttpResponseMessage> response = await _keycloakApi.LogoutAsync(_configuration["Keycloak:Realm"]!, parameters);
            return response.IsSuccessStatusCode;
        }
        catch (Refit.ApiException ex)
        {
            throw new Exception($"Logout failed: {ex.Message}");
        }
    }

    public async Task<ForgotPasswordResponseDto> SendPasswordResetEmailAsync(string email, CancellationToken cancellationToken)
    {
        try
        {
            string token = await GetAdminAccessTokenAsync();

            // First, find the user by email
            List<KeycloakUserDto> users = await _keycloakApi.GetUsersByEmailAsync(
                _configuration["Keycloak:Realm"]!,
                email,
                $"Bearer {token}"
            ).ConfigureAwait(false);

            if (!users.Any())
            {
                return new ForgotPasswordResponseDto
                {
                    Success = false,
                    Message = "User not found with the provided email address."
                };
            }

            User? user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == email, cancellationToken);

            if (user == null)
            {
                return new ForgotPasswordResponseDto
                {
                    Success = false,
                    Message = "User not found with the provided email address."
                };

            }
            string resetToken = _tokenService.CreateOnboardingToken(user);
            var emailMessage = new EmailMessage
            {
                Email = user.Email,
                Title = "Reset Your Password",
                Name = user.FirstName + " " + user.LastName,
                Description = "It looks like you requested to reset your password. No worries — we're here to help! Click the button below to securely set a new password. If you didn’t request this, you can safely ignore this email.",
                EmailButton = true,
                ButtonLink = $"{_configuration["Frontend:BaseUrl"]}/auth/password/reset-password?token={resetToken}",
                ButtonText = "Reset Password"
            };

            await _emailService.SendEmailAsync(emailMessage);

            return new ForgotPasswordResponseDto
            {
                Success = true,
                Message = "Password reset email sent successfully."
            };

            // KeycloakUserDto user = users[0];
            // var actions = new List<string> { "UPDATE_PASSWORD" };

            // Refit.ApiResponse<HttpResponseMessage> response = await _keycloakApi.SendResetPasswordEmailAsync(
            //     _configuration["Keycloak:Realm"]!,
            //     user.Id,
            //     actions,
            //     $"Bearer {token}",
            //     _configuration["Keycloak:ClientId"]!
            // // _configuration["Keycloak:RedirectUri"] ?? ""
            // );

            // if (response.IsSuccessStatusCode)
            // {
            //     return new ForgotPasswordResponseDto
            //     {
            //         Success = true,
            //         Message = "Password reset email sent successfully."
            //     };
            // }
            // else
            // {
            //     return new ForgotPasswordResponseDto
            //     {
            //         Success = false,
            //         Message = "Failed to send password reset email."
            //     };
            // }
        }
        catch (Exception ex)
        {
            return new ForgotPasswordResponseDto
            {
                Success = false,
                Message = $"Error sending password reset email: {ex.Message}"
            };
        }
    }

    public async Task<bool> ValidateTokenAsync(string accessToken)
    {
        var parameters = new Dictionary<string, string>
        {
            { "token", accessToken },
            { "client_id", _configuration["Keycloak:ClientId"]! },
            { "client_secret", _configuration["Keycloak:ClientSecret"]! }
        };

        try
        {
            TokenIntrospectionResponseDto response = await _keycloakApi.IntrospectTokenAsync(_configuration["Keycloak:Realm"]!, parameters);
            return response.Active;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<KeycloakUserDto?> GetUserByEmailAsync(string email)
    {
        try
        {
            string token = await GetAdminAccessTokenAsync();
            List<KeycloakUserDto> users = await _keycloakApi.GetUsersByEmailAsync(
                _configuration["Keycloak:Realm"]!,
                email,
                $"Bearer {token}"
            );

            return users.FirstOrDefault();
        }
        catch (Exception)
        {
            return null;
        }
    }

    public async Task<bool> SendUserSetupEmailAsync(string keycloakUserId)
    {
        try
        {
            string token = await GetAdminAccessTokenAsync();

            var actions = new List<string>
            {
                "UPDATE_PASSWORD",
            };

            Refit.ApiResponse<HttpResponseMessage> response = await _keycloakApi.SendRequiredActionsEmailAsync(
                _configuration["Keycloak:Realm"]!,
                keycloakUserId,
                actions,
                $"Bearer {token}",
                _configuration["Keycloak:ClientId"]!,
                86400 // 24 hours lifespan
            );

            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error sending setup email: {ex.Message}");
            return false;
        }
    }

    public async Task<SetPasswordResponseDto> ChangeUserPasswordAsync(string currentPassword, string newPassword, string userAccessToken)
    {
        try
        {
            var request = new ChangePasswordRequest
            {
                CurrentPassword = currentPassword,
                NewPassword = newPassword,
                Confirmation = newPassword
            };

            Refit.ApiResponse<HttpResponseMessage> response = await _keycloakApi.ChangePasswordAsync(
                _configuration["Keycloak:Realm"]!,
                request,
                $"Bearer {userAccessToken}"
            );

            if (response.IsSuccessStatusCode)
            {
                return new SetPasswordResponseDto
                {
                    Success = true,
                    Message = "Password changed successfully."
                };
            }
            else
            {
                return new SetPasswordResponseDto
                {
                    Success = false,
                    Message = "Failed to change password. Please check your current password."
                };
            }
        }
        catch (Refit.ApiException ex) when (ex.StatusCode == System.Net.HttpStatusCode.BadRequest)
        {
            return new SetPasswordResponseDto
            {
                Success = false,
                Message = "Invalid current password or new password doesn't meet requirements."
            };
        }
        catch (Exception ex)
        {
            return new SetPasswordResponseDto
            {
                Success = false,
                Message = $"Error changing password: {ex.Message}"
            };
        }
    }

    public async Task ResetPasswordAsync(Guid KeycloakUserId, string password)
    {
        string token = await GetAdminAccessTokenAsync();
        var request = new PasswordResetRequest
        {
            Temporary = false,
            Type = "password",
            Value = password
        };

        string authorizationHeader = $"Bearer {token}";
        try
        {
            await _keycloakApi.ResetPasswordAsync(_configuration["Keycloak:Realm"]!, KeycloakUserId.ToString(), request, authorizationHeader);
            Console.WriteLine("Password reset successfully for: ." + KeycloakUserId);
        }
        catch (ApiException ex)
        {
            Console.WriteLine($"Failed to reset password: {ex.Message}");
            throw;
        }
    }
}
