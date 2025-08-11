using Application.Abstractions.Data;
using Application.Interfaces.Services;
using Domain.Roles;
using iText.Commons.Actions.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Data;

public sealed class DataSeeder(
    IApplicationDbContext context,
    ILogger<DataSeeder> logger, KeycloakRolesService _keycloakRolesService)
{
    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await SeedPermissions(cancellationToken);
            await SeedRoles(cancellationToken);
            await SyncToKeycloak();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while seeding data");
        }
    }

    private async Task SeedPermissions(CancellationToken cancellationToken)
    {
        var permissionsToSeed = new List<(string Name, string Description)>
        {
            ("CreateSchool", "Permission to create a school"),
            //("users:read", "Read users information"),
            //("users:create", "Create new users"),
            //("users:update", "Update users"),
            //("users:delete", "Delete users"),
            //("roles:read", "Read roles information"),
            //("roles:create", "Create new roles"),
            //("roles:update", "Update roles"),
            //("roles:delete", "Delete roles"),
            //("dashboard:view", "View dashboard"),
            //("reports:generate", "Generate reports"),
            //("care:read", "Read care information"),
            //("care:create", "Create care records"),
            //("care:update", "Update care records"),
            //("profile:read", "Read own profile"),
            //("profile:update", "Update own profile")
        };

        List<string> existingPermissions = await context.Permissions
            .Select(p => p.Name)
            .ToListAsync(cancellationToken);

        var newPermissions = permissionsToSeed
            .Where(p => !existingPermissions.Contains(p.Name))
            .Select(p => Permission.Create(p.Name, p.Description))
            .ToList();

        if (newPermissions.Any())
        {
            await context.Permissions.AddRangeAsync(newPermissions, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
            logger.LogInformation("Added {Count} new permissions", newPermissions.Count);
        }
    }

    private async Task SeedRoles(CancellationToken cancellationToken)
    {
        List<Permission> permissions = await context.Permissions.ToListAsync(cancellationToken);
        List<string> existingRoles = await context.Roles
            .Select(r => r.Name)
            .ToListAsync(cancellationToken);

        // Define roles and their permissions
        var rolesToSeed = new Dictionary<string, (string Description, List<string> PermissionNames)>
        {
            {
                "Super admin",
                ("System administrator with full access", new List<string>
                {
                    "CreateSchool"
                })
            },
            //{
            //    "Manager",
            //    ("Manager with limited administrative access", new List<string>
            //    {
            //        "users:read", "users:create", "dashboard:view", "reports:generate", "care:read"
            //    })
            //},
            //{
            //    "Care Giver",
            //    ("Care Giver with standard privileges", new List<string>
            //    {
            //        "users:read", "dashboard:view", "care:read", "care:create", "care:update", "profile:read", "profile:update"
            //    })
            //},
            //{
            //    "User",
            //    ("Regular system user", new List<string>
            //    {
            //        "dashboard:view", "profile:read", "profile:update"
            //    })
            //},
            //{
            //    "Service User",
            //    ("Service user with limited access to their own information", new List<string>
            //    {
            //        "dashboard:view", "profile:read", "profile:update", "care:read"
            //    })
            //}
        };

        var rolesToAdd = new List<Role>();

        foreach (KeyValuePair<string, (string Description, List<string> PermissionNames)> roleData in rolesToSeed)
        {
            string roleName = roleData.Key;
            (string description, List<string> permissionNames) = roleData.Value;

            // Skip if role already exists
            if (existingRoles.Contains(roleName))
            {
                logger.LogInformation("Role '{RoleName}' already exists, skipping", roleName);
                continue;
            }

            var role = Role.Create(roleName, description);

            // Add permissions to role
            foreach (string permissionName in permissionNames)
            {
                Permission? permission = permissions.FirstOrDefault(p => p.Name == permissionName);
                if (permission != null)
                {
                    role.AddPermission(permission);
                }
                else
                {
                    logger.LogWarning("Permission '{PermissionName}' not found for role '{RoleName}'", 
                        permissionName, roleName);
                }
            }

            rolesToAdd.Add(role);
        }

        if (rolesToAdd.Any())
        {
            await context.Roles.AddRangeAsync(rolesToAdd, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
            logger.LogInformation("Added {Count} new roles", rolesToAdd.Count);
        }
        else
        {
            logger.LogInformation("No new roles to add");
        }
    }

    private async Task SyncToKeycloak()
    {
        try
        {
            logger.LogInformation("Starting synchronization with Keycloak");

            List<Role> roles = await context.Roles.Include(r => r.Permissions).ToListAsync();
            List<Permission> permissions = await context.Permissions.ToListAsync();

            bool updated = false;

            foreach (Role role in roles)
            {
                // Check if the role exists in Keycloak before syncing
                Application.Interfaces.KeycloakRoleDto? existingKeycloakRole = await _keycloakRolesService.GetRealmRoleByNameAsync(role.Name);

                if (existingKeycloakRole != null)
                {
                    logger.LogInformation("Role '{RoleName}' already exists in Keycloak with ID '{KeycloakId}'.",
                        role.Name, existingKeycloakRole.Id);

                    // Update KeycloakId in local DB if missing or outdated
                    if (string.IsNullOrWhiteSpace(role.KeycloakId) || role.KeycloakId != existingKeycloakRole.Id)
                    {
                        role.KeycloakId = existingKeycloakRole.Id;
                        updated = true;
                    }
                }
                else
                {
                    // Role does not exist ? create it
                    bool created = await _keycloakRolesService.CreateRealmRoleAsync(role.Name, role.Description);
                    if (created)
                    {
                        logger.LogInformation("Created role '{RoleName}' in Keycloak.", role.Name);

                        // Retrieve it again to get its ID
                        Application.Interfaces.KeycloakRoleDto? newKeycloakRole = await _keycloakRolesService.GetRealmRoleByNameAsync(role.Name);
                        if (newKeycloakRole != null)
                        {
                            role.KeycloakId = newKeycloakRole.Id;
                            updated = true;
                        }
                    }
                    else
                    {
                        logger.LogWarning("Failed to create role '{RoleName}' in Keycloak.", role.Name);
                    }
                }
            }

            // Sync permissions as well
            foreach (Permission permission in permissions)
            {
                Application.Interfaces.KeycloakRoleDto? existingKeycloakPermission = await _keycloakRolesService.GetRealmRoleByNameAsync(permission.Name);

                if (existingKeycloakPermission != null)
                {
                    if (string.IsNullOrWhiteSpace(permission.KeycloakId) || permission.KeycloakId != existingKeycloakPermission.Id)
                    {
                        permission.KeycloakId = existingKeycloakPermission.Id;
                        updated = true;
                    }
                }
                else
                {
                    bool created = await _keycloakRolesService.CreateRealmRoleAsync(permission.Name, permission.Description);
                    if (created)
                    {
                        Application.Interfaces.KeycloakRoleDto? newPermissionRole = await _keycloakRolesService.GetRealmRoleByNameAsync(permission.Name);
                        if (newPermissionRole != null)
                        {
                            permission.KeycloakId = newPermissionRole.Id;
                            updated = true;
                        }
                    }
                }
            }

            if (updated)
            {
                await context.SaveChangesAsync();
                logger.LogInformation("Updated KeycloakId for roles and permissions after sync.");
            }

            logger.LogInformation("Finished synchronization with Keycloak.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to synchronize with Keycloak");
            // Don't rethrow - we want local seeding to succeed even if Keycloak sync fails
        }
    }


    // Method to sync specific role to Keycloak
    public async Task<bool> SyncRoleToKeycloakAsync(string roleName)
    {
        try
        {
            Role? role = await context.Roles
                .Include(r => r.Permissions)
                .FirstOrDefaultAsync(r => r.Name == roleName);

            if (role == null)
            {
                logger.LogWarning("Role '{RoleName}' not found in local database", roleName);
                return false;
            }

            // Check if role exists in Keycloak
            Application.Interfaces.KeycloakRoleDto? keycloakRole = await _keycloakRolesService.GetRealmRoleByNameAsync(roleName);
            bool result;

            if (keycloakRole != null)
            {
                // Update existing role
                result = await _keycloakRolesService.UpdateRealmRoleAsync(role.Name, role.Description);
            }
            else
            {
                // Create new role
                result = await _keycloakRolesService.CreateRealmRoleAsync(role.Name, role.Description);
            }

            if (result && role.Permissions.Any())
            {
                var permissionRoleNames = role.Permissions.Select(p => p.Name).ToList();

                foreach (Permission permission in role.Permissions)
                {
                    string permissionRoleName = permission.Name;
                    Application.Interfaces.KeycloakRoleDto? existingPermissionRole = await _keycloakRolesService.GetRealmRoleByNameAsync(permissionRoleName);

                    if (existingPermissionRole == null)
                    {
                        await _keycloakRolesService.CreateRealmRoleAsync(permissionRoleName, permission.Description);
                    }
                }

                await _keycloakRolesService.AddCompositeRolesAsync(role.Name, permissionRoleNames);
            }

            logger.LogInformation("Successfully synced role '{RoleName}' to Keycloak", roleName);
            return result;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to sync role '{RoleName}' to Keycloak", roleName);
            return false;
        }
    }

    // Method to remove role from Keycloak
    public async Task<bool> RemoveRoleFromKeycloakAsync(string roleName)
    {
        try
        {
            bool result = await _keycloakRolesService.DeleteRealmRoleAsync(roleName);

            if (result)
            {
                logger.LogInformation("Successfully removed role '{RoleName}' from Keycloak", roleName);
            }
            else
            {
                logger.LogWarning("Failed to remove role '{RoleName}' from Keycloak", roleName);
            }

            return result;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error removing role '{RoleName}' from Keycloak", roleName);
            return false;
        }
    }

    // Method to verify sync status
    public async Task<Dictionary<string, bool>> VerifySyncStatusAsync()
    {
        var syncStatus = new Dictionary<string, bool>();

        try
        {
            List<string> localRoles = await context.Roles.Select(r => r.Name).ToListAsync();
            List<Application.Interfaces.KeycloakRoleDto> keycloakRoles = await _keycloakRolesService.GetAllRealmRolesAsync();
            var keycloakRoleNames = keycloakRoles.Select(r => r.Name).ToHashSet();

            foreach (string? localRoleName in localRoles)
            {
                syncStatus[localRoleName] = keycloakRoleNames.Contains(localRoleName);
            }

            logger.LogInformation("Verified sync status for {Count} roles", syncStatus.Count);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to verify sync status");
        }

        return syncStatus;
    }
}

 // ADMINISTRATOR,
        // CARE_GIVER,
        // SCHEDULER,
        // SERVICE_USER,
        // SUPER_ADMIN,
        // REGULATORY_AUDITOR,
        // SERVICE_COORDINATOR,
        // MEDICATION_SUPPORT_WORKER
