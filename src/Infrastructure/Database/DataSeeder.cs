using System.Threading;
using Application.Abstractions.Data;
using Application.Interfaces.Services;
using Domain.HelpRequests;
using Domain.Roles;
using iText.Commons.Actions.Contexts;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Serilog.Core;
using SharedKernel.Enums;

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
            await SeedHelpRequests(cancellationToken);
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

    //private async Task SeedHelpRequests(CancellationToken cancellationToken)
    //{
    //    List<string?> existingTicketIds = await context.HelpRequests
    //        .Select(hr => hr.TicketId)
    //        .ToListAsync(cancellationToken);



    //    var helpRequestsToAdd = new List<HelpRequests>();

    //    foreach ((string TicketId, HelpStatus Status, HelpCategory Category, string UserProfilePic, string UserName, string TenantHelpRequestId, string SchoolId, List<(string Title, List<string> Attachments)> Messages) helpRequestData in helpRequestsToSeed)
    //    {
    //        if (existingTicketIds.Contains(helpRequestData.TicketId))
    //        {
    //            logger.LogInformation("Help request with TicketId '{TicketId}' already exists, skipping", helpRequestData.TicketId);
    //            continue;
    //        }
    //        var helpRequest = new HelpRequests
    //        {
    //            TicketId = helpRequestData.TicketId,
    //            Status = helpRequestData.Status,
    //            Category = helpRequestData.Category,
    //            UserProfilePic = helpRequestData.UserProfilePic,
    //            UserName = helpRequestData.UserName,
    //            TenantHelpRequestId = helpRequestData.TenantHelpRequestId,
    //            SchoolId = helpRequestData.SchoolId,
    //            Messages = new List<HelpRequestMessages>()
    //        };
    //        foreach ((string Title, List<string> Attachments) messageData in helpRequestData.Messages)
    //        {
    //            var message = new HelpRequestMessages
    //            {
    //                Title = messageData.Title,
    //                Attachments = messageData.Attachments
    //            };
    //            helpRequest.Messages.Add(message);
    //        }

    //        helpRequestsToAdd.Add(helpRequest);
    //    }

    //    if (helpRequestsToAdd.Any())
    //    {
    //        await context.HelpRequests.AddRangeAsync(helpRequestsToAdd, cancellationToken);
    //        await context.SaveChangesAsync(cancellationToken);
    //        logger.LogInformation("Added {Count} new help requests with their messages", helpRequestsToAdd.Count);
    //    }
    //    else
    //    {
    //        logger.LogInformation("No new help requests to add");
    //    }
    //}


    private async Task SeedHelpRequests(CancellationToken cancellationToken)
    {
        // Check for existing help requests to avoid duplicates
        List<string?> existingTicketIds = await context.HelpRequests
             .Select(hr => hr.TicketId)
             .ToListAsync(cancellationToken);

        // Define help requests to seed
        var helpRequestsToSeed = new List<(string TicketId, HelpStatus Status, HelpCategory Category, string UserProfilePic, string UserName, string TenantHelpRequestId, string SchoolId, List<(string Title, List<string> Attachments, string MessageUserName, string MessageUserProfilePic)> Messages)>
        {
            (
                TicketId: "TICKET-001",
                Status: HelpStatus.OPEN_REQUEST,
                Category: HelpCategory.TECHNICAL_ISSUE,
                UserProfilePic: "https://example.com/profiles/user1.jpg",
                UserName: "John Doe",
                TenantHelpRequestId: "TENANT-001",
                SchoolId: "SCHOOL-001",
                Messages: new List<(string Title, List<string> Attachments, string MessageUserName, string MessageUserProfilePic)>
                {
                    ("Login Issue", new List<string> { "https://example.com/attachments/screenshot1.png", "https://example.com/attachments/error_log.txt" }, "John Doe", "https://example.com/profiles/user1.jpg"),
                    ("Follow-up on Login Issue", new List<string> { "https://example.com/attachments/screenshot2.png" }, "John Doe", "https://example.com/profiles/user1.jpg"),
                    ("Request for Admin Assistance", new List<string>(), "Admin Support", "https://example.com/profiles/admin1.jpg"),
                    ("Additional Details", new List<string> { "https://example.com/attachments/config.json" }, "John Doe", "https://example.com/profiles/user1.jpg")
                }
            ),
            (
                TicketId: "TICKET-002",
                Status: HelpStatus.IN_PROGRESS,
                Category: HelpCategory.TECHNICAL_ISSUE,
                UserProfilePic: "https://example.com/profiles/user2.jpg",
                UserName: "Jane Smith",
                TenantHelpRequestId: "TENANT-002",
                SchoolId: "SCHOOL-002",
                Messages: new List<(string Title, List<string> Attachments, string MessageUserName, string MessageUserProfilePic)>
                {
                    ("Billing Query", new List<string> { "https://example.com/attachments/invoice.pdf" }, "Jane Smith", "https://example.com/profiles/user2.jpg"),
                    ("Payment Confirmation", new List<string> { "https://example.com/attachments/receipt.pdf" }, "Jane Smith", "https://example.com/profiles/user2.jpg"),
                    ("Clarification Needed", new List<string>(), "Billing Team", "https://example.com/profiles/billing1.jpg")
                }
            ),
            (
                TicketId: "TICKET-003",
                Status: HelpStatus.RESOLVED,
                Category: HelpCategory.OTHER,
                UserProfilePic: "https://example.com/profiles/user3.jpg",
                UserName: "Alice Johnson",
                TenantHelpRequestId: "TENANT-003",
                SchoolId: "SCHOOL-003",
                Messages: new List<(string Title, List<string> Attachments, string MessageUserName, string MessageUserProfilePic)>
                {
                    ("Account Activation Request", new List<string> { "https://example.com/attachments/id_scan.jpg" }, "Alice Johnson", "https://example.com/profiles/user3.jpg"),
                    ("Verification Follow-up", new List<string>(), "Alice Johnson", "https://example.com/profiles/user3.jpg"),
                    ("Issue Resolved", new List<string> { "https://example.com/attachments/confirmation_email.pdf" }, "Admin Support", "https://example.com/profiles/admin1.jpg"),
                    ("Feedback on Resolution", new List<string>(), "Alice Johnson", "https://example.com/profiles/user3.jpg")
                }
            ),
            (
                TicketId: "TICKET-004",
                Status: HelpStatus.OPEN_REQUEST,
                Category: HelpCategory.TECHNICAL_ISSUE,
                UserProfilePic: "https://example.com/profiles/user4.jpg",
                UserName: "Bob Wilson",
                TenantHelpRequestId: "TENANT-004",
                SchoolId: "SCHOOL-004",
                Messages: new List<(string Title, List<string> Attachments, string MessageUserName, string MessageUserProfilePic)>
                {
                    ("Network Connectivity Issue", new List<string> { "https://example.com/attachments/network_log.txt" }, "Bob Wilson", "https://example.com/profiles/user4.jpg"),
                    ("Ping Test Results", new List<string> { "https://example.com/attachments/ping_results.csv" }, "Bob Wilson", "https://example.com/profiles/user4.jpg"),
                    ("Router Configuration", new List<string> { "https://example.com/attachments/router_config.pdf" }, "Tech Support", "https://example.com/profiles/tech1.jpg")
                }
            ),
            (
                TicketId: "TICKET-005",
                Status: HelpStatus.IN_PROGRESS,
                Category: HelpCategory.OTHER,
                UserProfilePic: "https://example.com/profiles/user5.jpg",
                UserName: "Emma Davis",
                TenantHelpRequestId: "TENANT-005",
                SchoolId: "SCHOOL-005",
                Messages: new List<(string Title, List<string> Attachments, string MessageUserName, string MessageUserProfilePic)>
                {
                    ("General Inquiry", new List<string>(), "Emma Davis", "https://example.com/profiles/user5.jpg"),
                    ("Request for Training Materials", new List<string> { "https://example.com/attachments/training_request_form.pdf" }, "Emma Davis", "https://example.com/profiles/user5.jpg"),
                    ("Follow-up on Training", new List<string>(), "Training Team", "https://example.com/profiles/trainer1.jpg"),
                    ("Feedback on Materials", new List<string> { "https://example.com/attachments/feedback_form.docx" }, "Emma Davis", "https://example.com/profiles/user5.jpg")
                }
            ),
            (
                TicketId: "TICKET-006",
                Status: HelpStatus.OPEN_REQUEST,
                Category: HelpCategory.TECHNICAL_ISSUE,
                UserProfilePic: "https://example.com/profiles/user6.jpg",
                UserName: "Michael Brown",
                TenantHelpRequestId: "TENANT-006",
                SchoolId: "SCHOOL-006",
                Messages: new List<(string Title, List<string> Attachments, string MessageUserName, string MessageUserProfilePic)>
                {
                    ("Software Crash Report", new List<string> { "https://example.com/attachments/crash_log.txt", "https://example.com/attachments/screenshot3.png" }, "Michael Brown", "https://example.com/profiles/user6.jpg"),
                    ("Attempted Fixes", new List<string>(), "Michael Brown", "https://example.com/profiles/user6.jpg"),
                    ("Request for Patch", new List<string>(), "Tech Support", "https://example.com/profiles/tech1.jpg")
                }
            ),
            (
                TicketId: "TICKET-007",
                Status: HelpStatus.RESOLVED,
                Category: HelpCategory.BUG_REPORT,
                UserProfilePic: "https://example.com/profiles/user7.jpg",
                UserName: "Sarah Miller",
                TenantHelpRequestId: "TENANT-007",
                SchoolId: "SCHOOL-007",
                Messages: new List<(string Title, List<string> Attachments, string MessageUserName, string MessageUserProfilePic)>
                {
                    ("Overcharge Dispute", new List<string> { "https://example.com/attachments/invoice_dispute.pdf" }, "Sarah Miller", "https://example.com/profiles/user7.jpg"),
                    ("Response from Billing Team", new List<string> { "https://example.com/attachments/billing_response.pdf" }, "Billing Team", "https://example.com/profiles/billing1.jpg"),
                    ("Resolution Confirmation", new List<string>(), "Sarah Miller", "https://example.com/profiles/user7.jpg")
                }
            ),
            (
                TicketId: "TICKET-008",
                Status: HelpStatus.IN_PROGRESS,
                Category: HelpCategory.BUG_REPORT,
                UserProfilePic: "https://example.com/profiles/user8.jpg",
                UserName: "David Lee",
                TenantHelpRequestId: "TENANT-008",
                SchoolId: "SCHOOL-008",
                Messages: new List<(string Title, List<string> Attachments, string MessageUserName, string MessageUserProfilePic)>
                {
                    ("Password Reset Request", new List<string>(), "David Lee", "https://example.com/profiles/user8.jpg"),
                    ("Security Question Issue", new List<string> { "https://example.com/attachments/security_question_screenshot.png" }, "David Lee", "https://example.com/profiles/user8.jpg"),
                    ("Admin Follow-up", new List<string>(), "Admin Support", "https://example.com/profiles/admin1.jpg"),
                    ("Temporary Access Request", new List<string> { "https://example.com/attachments/temp_access_form.pdf" }, "David Lee", "https://example.com/profiles/user8.jpg")
                }
            ),
            (
                TicketId: "TICKET-009",
                Status: HelpStatus.OPEN_REQUEST,
                Category: HelpCategory.TECHNICAL_ISSUE,
                UserProfilePic: "https://example.com/profiles/user9.jpg",
                UserName: "Lisa Taylor",
                TenantHelpRequestId: "TENANT-009",
                SchoolId: "SCHOOL-009",
                Messages: new List<(string Title, List<string> Attachments, string MessageUserName, string MessageUserProfilePic)>
                {
                    ("Database Access Error", new List<string> { "https://example.com/attachments/db_error_log.txt" }, "Lisa Taylor", "https://example.com/profiles/user9.jpg"),
                    ("Query Performance Issue", new List<string> { "https://example.com/attachments/query_plan.pdf" }, "Lisa Taylor", "https://example.com/profiles/user9.jpg")
                }
            ),
            (
                TicketId: "TICKET-010",
                Status: HelpStatus.IN_PROGRESS,
                Category: HelpCategory.OTHER,
                UserProfilePic: "https://example.com/profiles/user10.jpg",
                UserName: "Chris Evans",
                TenantHelpRequestId: "TENANT-010",
                SchoolId: "SCHOOL-010",
                Messages: new List<(string Title, List<string> Attachments, string MessageUserName, string MessageUserProfilePic)>
                {
                    ("Feature Request", new List<string> { "https://example.com/attachments/feature_proposal.docx" }, "Chris Evans", "https://example.com/profiles/user10.jpg"),
                    ("Clarification on Requirements", new List<string>(), "Chris Evans", "https://example.com/profiles/user10.jpg"),
                    ("Mockup Submission", new List<string> { "https://example.com/attachments/mockup.png" }, "Chris Evans", "https://example.com/profiles/user10.jpg"),
                    ("Feedback on Proposal", new List<string>(), "Product Team", "https://example.com/profiles/product1.jpg")
                }
            )
        };

        var helpRequestsToAdd = new List<HelpRequests>();

        foreach ((string TicketId, HelpStatus Status, HelpCategory Category, string UserProfilePic, string UserName, string TenantHelpRequestId, string SchoolId, List<(string Title, List<string> Attachments, string MessageUserName, string MessageUserProfilePic)> Messages) helpRequestData in helpRequestsToSeed)
        {
            // Skip if help request already exists
            if (existingTicketIds.Contains(helpRequestData.TicketId))
            {
                logger.LogInformation("Help request with TicketId '{TicketId}' already exists, skipping", helpRequestData.TicketId);
                continue;
            }

            // Create new help request
            var helpRequest = new HelpRequests
            {
                TicketId = helpRequestData.TicketId,
                Status = helpRequestData.Status,
                Category = helpRequestData.Category,
                UserProfilePic = helpRequestData.UserProfilePic,
                UserName = helpRequestData.UserName,
                TenantHelpRequestId = helpRequestData.TenantHelpRequestId,
                SchoolId = helpRequestData.SchoolId,
                Messages = new List<HelpRequestMessages>()
            };

            // Add messages to the help request
            foreach ((string Title, List<string> Attachments, string MessageUserName, string MessageUserProfilePic) messageData in helpRequestData.Messages)
            {
                var message = new HelpRequestMessages
                {
                    Title = messageData.Title,
                    Attachments = messageData.Attachments,
                    UserName = messageData.MessageUserName,
                    UserProfilePic = messageData.MessageUserProfilePic,
                };
                helpRequest.Messages.Add(message);
            }

            helpRequestsToAdd.Add(helpRequest);
        }

        if (helpRequestsToAdd.Any())
        {
            await context.HelpRequests.AddRangeAsync(helpRequestsToAdd, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
            logger.LogInformation("Added {Count} new help requests with their messages", helpRequestsToAdd.Count);
        }
        else
        {
            logger.LogInformation("No new help requests to add");
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
