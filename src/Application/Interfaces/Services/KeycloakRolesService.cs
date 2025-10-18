using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Abstractions.Authentication;
using Application.BackgroundJobs;
using Domain.Roles;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Application.Interfaces.Services;
public class KeycloakRolesService
{
    private readonly IKeycloakApi _keycloakApi;
    private readonly IConfiguration _configuration;
    private readonly KeycloakService _KeycloakService;
    private readonly IApplicationDbContext _dbContext;
    private readonly IUserContext _userContext;
    private readonly ILogger<KeycloakRolesService> _logger;

    public KeycloakRolesService(
        IKeycloakApi keycloakApi,
        IConfiguration configuration,
        IUserContext userContext,
        KeycloakService keycloakService,
        IApplicationDbContext dbContext,
        ILogger<KeycloakRolesService> logger)
    {
        _keycloakApi = keycloakApi;
        _configuration = configuration;
        _KeycloakService = keycloakService;
        _userContext = userContext;
        _dbContext = dbContext;
        _logger = logger;
    }


    public async Task<List<KeycloakRoleDto>> GetAllRealmRolesAsync()
    {
        string token = await _KeycloakService.GetAdminAccessTokenAsync();
        string realm = _configuration["Keycloak:Realm"]!;

        Refit.ApiResponse<List<KeycloakRoleDto>> response = await _keycloakApi.GetRealmRolesAsync(realm, $"Bearer {token}");

        if (response.IsSuccessStatusCode && response.Content != null)
        {
            return response.Content;
        }

        _logger.LogError("Failed to retrieve realm roles. Status: {Status}", response.StatusCode);
        return new List<KeycloakRoleDto>();
    }

    public async Task<KeycloakRoleDto?> GetRealmRoleByNameAsync(string roleName)
    {
        string token = await _KeycloakService.GetAdminAccessTokenAsync();
        string realm = _configuration["Keycloak:Realm"]!;

        Refit.ApiResponse<KeycloakRoleDto> response = await _keycloakApi.GetRealmRoleByNameAsync(realm, roleName, $"Bearer {token}");

        if (response.IsSuccessStatusCode && response.Content != null)
        {
            return response.Content;
        }

        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return null;
        }

        _logger.LogError("Failed to retrieve realm role '{RoleName}'. Status: {Status}", roleName, response.StatusCode);
        return null;
    }

    public async Task<bool> CreateRealmRoleAsync(string name, string description, bool isComposite = false)
    {
        string token = await _KeycloakService.GetAdminAccessTokenAsync();
        string realm = _configuration["Keycloak:Realm"]!;

        var createRoleDto = new CreateKeycloakRoleDto
        {
            Name = name,
            Description = description,
            Composite = isComposite
        };

        Refit.ApiResponse<HttpResponseMessage> response = await _keycloakApi.CreateRealmRoleAsync(realm, createRoleDto, $"Bearer {token}");

        if (response.IsSuccessStatusCode)
        {
            _logger.LogInformation("Successfully created realm role '{RoleName}'", name);
            return true;
        }

        _logger.LogError("Failed to create realm role '{RoleName}'. Status: {Status}", name, response.StatusCode);
        return false;
    }

    public async Task<bool> UpdateRealmRoleAsync(string roleName, string description)
    {
        string token = await _KeycloakService.GetAdminAccessTokenAsync();
        string realm = _configuration["Keycloak:Realm"]!;

        var updateRoleDto = new UpdateKeycloakRoleDto
        {
            Name = roleName,
            Description = description,
            Composite = false
        };

        Refit.ApiResponse<HttpResponseMessage> response = await _keycloakApi.UpdateRealmRoleAsync(realm, roleName, updateRoleDto, $"Bearer {token}");

        if (response.IsSuccessStatusCode)
        {
            _logger.LogInformation("Successfully updated realm role '{RoleName}'", roleName);
            return true;
        }

        _logger.LogError("Failed to update realm role '{RoleName}'. Status: {Status}", roleName, response.StatusCode);
        return false;
    }

    public async Task<bool> DeleteRealmRoleAsync(string roleName)
    {
        string token = await _KeycloakService.GetAdminAccessTokenAsync();
        string realm = _configuration["Keycloak:Realm"]!;

        Refit.ApiResponse<HttpResponseMessage> response = await _keycloakApi.DeleteRealmRoleAsync(realm, roleName, $"Bearer {token}");

        if (response.IsSuccessStatusCode)
        {
            _logger.LogInformation("Successfully deleted realm role '{RoleName}'", roleName);
            return true;
        }

        _logger.LogError("Failed to delete realm role '{RoleName}'. Status: {Status}", roleName, response.StatusCode);
        return false;
    }

    // Composite Roles Management
    public async Task<bool> AddCompositeRolesAsync(string parentRoleName, List<string> childRoleNames)
    {
        string token = await _KeycloakService.GetAdminAccessTokenAsync();
        string realm = _configuration["Keycloak:Realm"]!;

        // First, get the child roles
        var childRoles = new List<KeycloakRoleDto>();
        foreach (string childRoleName in childRoleNames)
        {
            KeycloakRoleDto? childRole = await GetRealmRoleByNameAsync(childRoleName);
            if (childRole != null)
            {
                childRoles.Add(childRole);
            }
            else
            {
                _logger.LogWarning("Child role '{ChildRoleName}' not found for composite role '{ParentRoleName}'",
                    childRoleName, parentRoleName);
            }
        }

        if (!childRoles.Any())
        {
            _logger.LogWarning("No valid child roles found for composite role '{ParentRoleName}'", parentRoleName);
            return false;
        }

        Refit.ApiResponse<HttpResponseMessage> response = await _keycloakApi.AddCompositeRolesAsync(realm, parentRoleName, childRoles, $"Bearer {token}");

        if (response.IsSuccessStatusCode)
        {
            _logger.LogInformation("Successfully added {Count} composite roles to '{ParentRoleName}'",
                childRoles.Count, parentRoleName);
            return true;
        }

        _logger.LogError("Failed to add composite roles to '{ParentRoleName}'. Status: {Status}",
            parentRoleName, response.StatusCode);
        return false;
    }

    // Sync local roles and permissions to Keycloak
    public async Task<bool> SyncRolesToKeycloakAsync()
    {
        try
        {
            _logger.LogInformation("Starting sync of roles to Keycloak");

            // Get all local roles with their permissions
            List<Role> localRoles = await _dbContext.Roles
                .Include(r => r.Permissions)
                .ToListAsync();

            // Get existing Keycloak roles
            List<KeycloakRoleDto> keycloakRoles = await GetAllRealmRolesAsync();
            var existingKeycloakRoleNames = keycloakRoles.Select(r => r.Name).ToHashSet();

            var syncResults = new List<bool>();

            // --- CREATE or UPDATE ---
            foreach (Role? localRole in localRoles)
            {
                bool result;
                if (existingKeycloakRoleNames.Contains(localRole.Name))
                {
                    // Update existing role
                    result = await UpdateRealmRoleAsync(localRole.Name, localRole.Description);
                }
                else
                {
                    // Create new role
                    result = await CreateRealmRoleAsync(localRole.Name, localRole.Description);
                }

                syncResults.Add(result);

                if (result)
                {
                    // Sync permissions as composite roles if needed
                    await SyncPermissionsAsCompositeRolesAsync(localRole);
                }
            }

            // --- DELETE missing roles in Keycloak ---
            var dbRoleNamesSet = localRoles.Select(r => r.Name).ToHashSet();
            var orphanRolesSet = existingKeycloakRoleNames.Except(dbRoleNamesSet).ToList();

            foreach (string? orphan in orphanRolesSet)
            {
                _logger.LogInformation("Deleting orphan role '{Role}' from Keycloak", orphan);
                bool deleteResult = await DeleteRealmRoleAsync(orphan);
                syncResults.Add(deleteResult);
            }

            bool overallSuccess = syncResults.All(r => r);
            _logger.LogInformation(
                "Sync completed. Success: {Success}. Synced {SyncedCount} roles. Deleted {DeletedCount} orphan roles.",
                overallSuccess,
                localRoles.Count,
                orphanRolesSet.Count
            );

            return overallSuccess;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while syncing roles to Keycloak");
            return false;
        }
    }

    private async Task SyncPermissionsAsCompositeRolesAsync(Role localRole)
    {
        if (!localRole.Permissions.Any())
        {
            return;
        }

        // First, ensure permission roles exist in Keycloak
        foreach (Permission permission in localRole.Permissions)
        {
            string permissionRoleName = permission.Name;
            KeycloakRoleDto? existingPermissionRole = await GetRealmRoleByNameAsync(permissionRoleName);

            if (existingPermissionRole == null)
            {
                await CreateRealmRoleAsync(permissionRoleName, permission.Description);
            }
        }

        // Add permissions as composite roles
        var permissionRoleNames = localRole.Permissions
            .Select(p => p.Name)
            .ToList();

        if (permissionRoleNames.Any())
        {
            await AddCompositeRolesAsync(localRole.Name, permissionRoleNames);
        }
    }

    public async Task<bool> AssignRoleToUserAsync(string userId, string roleName)
    {
        KeycloakRoleDto? role = await GetRealmRoleByNameAsync(roleName);
        if (role == null)
        {
            _logger.LogError("Role '{RoleName}' not found for user assignment", roleName);
            return false;
        }

        string token = await _KeycloakService.GetAdminAccessTokenAsync();
        string realm = _configuration["Keycloak:Realm"]!;

        Refit.ApiResponse<HttpResponseMessage> response = await _keycloakApi.AssignRealmRolesToUserAsync(
            realm, userId, new List<KeycloakRoleDto> { role }, $"Bearer {token}");

        if (response.IsSuccessStatusCode)
        {
            _logger.LogInformation("Successfully assigned role '{RoleName}' to user '{UserId}'", roleName, userId);
            return true;
        }

        _logger.LogError("Failed to assign role '{RoleName}' to user '{UserId}'. Status: {Status}",
            roleName, userId, response.StatusCode);
        return false;
    }

    public async Task<bool> RemoveRoleFromUserAsync(string userId, string roleName)
    {
        KeycloakRoleDto? role = await GetRealmRoleByNameAsync(roleName);
        if (role == null)
        {
            _logger.LogError("Role '{RoleName}' not found for user role removal", roleName);
            return false;
        }

        string token = await _KeycloakService.GetAdminAccessTokenAsync();
        string realm = _configuration["Keycloak:Realm"]!;

        Refit.ApiResponse<HttpResponseMessage> response = await _keycloakApi.RemoveRealmRolesFromUserAsync(
            realm, userId, new List<KeycloakRoleDto> { role }, $"Bearer {token}");

        if (response.IsSuccessStatusCode)
        {
            _logger.LogInformation("Successfully removed role '{RoleName}' from user '{UserId}'", roleName, userId);
            return true;
        }

        _logger.LogError("Failed to remove role '{RoleName}' from user '{UserId}'. Status: {Status}",
            roleName, userId, response.StatusCode);
        return false;
    }

    public async Task<List<KeycloakRoleDto>> GetUserRolesAsync(string userId)
    {
        string token = await _KeycloakService.GetAdminAccessTokenAsync();
        string realm = _configuration["Keycloak:Realm"]!;

        Refit.ApiResponse<List<KeycloakRoleDto>> response = await _keycloakApi.GetUserRealmRolesAsync(realm, userId, $"Bearer {token}");

        if (response.IsSuccessStatusCode && response.Content != null)
        {
            return response.Content;
        }

        _logger.LogError("Failed to retrieve roles for user '{UserId}'. Status: {Status}", userId, response.StatusCode);
        return new List<KeycloakRoleDto>();
    }

    public async Task<bool> UpdateAuthenticatedUserPasswordAsync(string currentPassword, string newPassword)
    {
        try
        {
            // Get the user's access token (not admin token)
            string token = await _KeycloakService.GetAdminAccessTokenAsync();
            string realm = _configuration["Keycloak:Realm"]!;

            var request = new AuthenticatedUserChangePasswordRequest
            {
                CurrentPassword = currentPassword,
                NewPassword = newPassword,
                Confirmation = newPassword // Confirmation must match new password
            };

            Refit.ApiResponse<HttpResponseMessage> response = await _keycloakApi
                .UpdateAuthenticatedUserPasswordAsync(realm, request, $"Bearer {token}");

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Successfully updated authenticated user's password");
                return true;
            }

            _logger.LogError("Failed to update authenticated user's password. Status: {Status}", response.StatusCode);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception occurred while updating authenticated user's password");
            return false;
        }
    }
    public async Task<List<KeycloakSessionDto>> GetUserSessionsAsync()
    {
        try
        {
            string? token = _userContext.AccessToken;
            string userId = _userContext.KeycloakId ?? string.Empty;
            string realm = _configuration["Keycloak:Realm"]!;

            List<KeycloakSessionDto> response = await _keycloakApi.GetUserSessionsAsync(realm, userId, $"Bearer {token}");

            if (response != null)
            {
                _logger.LogInformation("Successfully retrieved {SessionCount} sessions for authenticated user",
                    response.Count);
                return response;
            }

            _logger.LogWarning("No sessions found for authenticated user");
            return new List<KeycloakSessionDto>();
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogError(ex, "Unauthorized access when retrieving user sessions");
            return new List<KeycloakSessionDto>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving user sessions");
            return new List<KeycloakSessionDto>();
        }
    }
}
