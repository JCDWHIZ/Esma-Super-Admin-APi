namespace Application.Interfaces;

public interface IKeycloakRolesService
{
    Task<bool> SyncRolesToKeycloakAsync();
    Task<KeycloakRoleDto?> GetRealmRoleByNameAsync(string roleName);
    Task<bool> CreateRealmRoleAsync(string name, string description, bool isComposite = false);
    Task<bool> UpdateRealmRoleAsync(string roleName, string description);
    Task<bool> DeleteRealmRoleAsync(string roleName);
    Task<bool> AddCompositeRolesAsync(string parentRoleName, List<string> childRoleNames);
    Task<List<KeycloakRoleDto>> GetAllRealmRolesAsync();
    Task<bool> AssignRoleToUserAsync(string userId, string roleName);
    Task<bool> RemoveRoleFromUserAsync(string userId, string roleName);
    Task<List<KeycloakRoleDto>> GetUserRolesAsync(string userId);
    Task<List<KeycloakSessionDto>> GetUserSessionsAsync();
}

