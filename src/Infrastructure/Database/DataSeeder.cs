using System.Data;
using Application.Abstractions.Data;
using Application.Interfaces;
using Domain.HelpRequests;
using Domain.Roles;
using Domain.Schools;
using Domain.Templates;
using Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SharedKernel.Enums;

namespace Infrastructure.Data;

public sealed class DataSeeder(
    IApplicationDbContext context,
    ILogger<DataSeeder> logger, IKeycloakRolesService _keycloakRolesService)
{
    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await SeedPermissions(cancellationToken);
            await SeedSchoolModules(cancellationToken);
            await SeedRoles(cancellationToken);
            await SeedSuperadmin(cancellationToken);
            //await SyncToKeycloak();
            await SeedHelpRequests(cancellationToken);
            await SeedTemplates(cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while seeding data");
        }
    }
    private async Task SeedSchoolModules(CancellationToken cancellationToken)
    {
        var modulesToSeed = new List<SchoolModule>
        {
            new() { Name = "Teachers", Key = "TEACHERS", Description = "Manage teachers and teacher records." },
            new() { Name = "Dashboard", Key = "DASHBOARD", Description = "View school analytics and high-level metrics." },
            new() { Name = "Settings", Key = "SETTINGS", Description = "Configure school-level settings and preferences." },
            new() { Name = "Students", Key = "STUDENTS", Description = "Manage student profiles and enrollment data." },
            new() { Name = "Parents", Key = "PARENTS", Description = "Manage parent accounts and linked students." },
            new() { Name = "Support", Key = "SUPPORT", Description = "Access support tickets and help desk tools." },
            new() { Name = "Suggestion", Key = "SUGGESTION", Description = "Submit and review suggestions." },
            new() { Name = "Classroom Management", Key = "CLASSROOMMANAGEMENT", Description = "Manage classrooms and class assignments." },
            new() { Name = "Virtual Classroom", Key = "VIRTUALCLASSROOM", Description = "Conduct and manage virtual classroom activities." },
            new() { Name = "Assignment", Key = "ASSIGNMENT", Description = "Create, assign, and review assignments." },
            new() { Name = "Virtual Meeting", Key = "VIRTUALMEETING", Description = "Schedule and manage virtual meetings." },
            new() { Name = "Exams", Key = "EXAMS", Description = "Manage exam setup, schedules, and grading." },
            new() { Name = "Lesson Plan", Key = "LESSONPLAN", Description = "Create and manage lesson plans." },
            new() { Name = "Admissions", Key = "ADMISSIONS", Description = "Manage admission applications and onboarding." },
            new() { Name = "Library", Key = "LIBRARY", Description = "Manage digital and physical library resources." },
            new() { Name = "Calendar", Key = "CALENDAR", Description = "Manage school events and schedules." },
            new() { Name = "Fees", Key = "FEES", Description = "Track fee setup, payments, and balances." },
            new() { Name = "Account Management", Key = "ACCOUNTMANAGEMENT", Description = "Manage school accounts and account settings." },
            new() { Name = "Broadcast", Key = "BROADCAST", Description = "Send announcements to school users." },
            new() { Name = "Messaging", Key = "MESSAGING", Description = "Access internal messaging and communication." },
            new() { Name = "Configuration", Key = "CONFIGURATION", Description = "Manage advanced system configurations." },
            new() { Name = "Audit", Key = "AUDIT", Description = "Review audit logs and compliance activities." }
        };

        List<string> existingKeys = await context.SchoolModules
            .Select(m => m.Key)
            .ToListAsync(cancellationToken);

        var newModules = modulesToSeed
            .Where(m => !existingKeys.Contains(m.Key))
            .ToList();

        if (newModules.Any())
        {
            await context.SchoolModules.AddRangeAsync(newModules, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
            logger.LogInformation("Added {Count} school modules", newModules.Count);
        }
    }
    private async Task SeedTemplates(CancellationToken cancellationToken)
    {
        var templatesToSeed = new List<Template>
        {
            Template.Create(
                "School admin Created",
                "Hi *username*, welcome to *schoolName*!",
                TriggerType.WELCOME_SCHOOLADMIN_EMAIL,
                new List<string> { "username", "schoolName" }
            ),
            Template.Create(
                "Admin created",
                "Hi *username*, welcome to elsoft!",
                TriggerType.WELCOME_ADMIN_EMAIL,
                new List<string> { "username" }
            ),
            Template.Create(
                "Password Reset",
                "Hello *username*, click here to reset your password: *resetLink*",
                TriggerType.PASSWORD_RESET,
                new List<string> { "username", "resetLink" }
            ),
            Template.Create(
                "Subscription Expired",
                "Dear *username*, your subscription expired on *expiryDate*.",
                TriggerType.SUBSCRIPTION_EXPIRED,
                new List<string> { "username", "expiryDate" }
            )
        };

        List<TriggerType> existingTriggers = await context.Templates
            .Select(t => t.TemplateTrigger)
            .ToListAsync(cancellationToken);

        var newTemplates = templatesToSeed
            .Where(t => !existingTriggers.Contains(t.TemplateTrigger))
            .ToList();

        if (newTemplates.Any())
        {
            await context.Templates.AddRangeAsync(newTemplates, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
            logger.LogInformation("Added {Count} new templates", newTemplates.Count);
        }
    }

    private async Task SeedSuperadmin(CancellationToken cancellationToken)
    {
        if (await context.Users.AnyAsync(u => u.Email == "elsoftadmin@gmail.com", cancellationToken))
        {
            return;
        }

        Role? superadminRole = await context.Roles.FirstOrDefaultAsync(r => r.Name == "Super admin", cancellationToken);
        if (superadminRole == null)
        {
            return;
        }

        var newUser = new User
        {
            Email = "elsoftadmin@gmail.com",
            RoleId = superadminRole.Id, // Use the fetched or created role ID
            Username = "elsoftadmin",
            KeycloakUserId = new Guid("883bd72d-8be8-45e5-8cbe-36e6486ed6f4"),
            FirstName = "elsoft",
            LastName = "admin",
            ProfilePic = "string",
            PhoneNumber = null
        };

        context.Users.Add(newUser);
        await context.SaveChangesAsync(cancellationToken);
    }


    private async Task SeedPermissions(CancellationToken cancellationToken)
    {
        var permissionsToSeed = new List<(string Name, string Description)>
        {
            ("dashboard_view", "Permission to view dashboard"),
            ("school_create", "Permission to create a school"),
            ("school_view", "Permission to view schools"),
            ("school_delete", "Permission to delete school"),
            ("school_edit", "Permission to edit school"),
            ("school_approve", "Permission to approve a school"),
            ("school_subscription_view", "Permission to view schools subscription"),
            ("school_subscription_edit", "Permission to update schools subscription"),
            ("admin_view", "Permission to view admins"),
            ("admin_edit", "Permission to edit admin information"),
            ("admin_invite", "Permission to invite an admin"),
            ("admin_delete", "Permission to delete an admin"),
            ("blog_publish", "Permission to publish blogs"),
            ("blog_update", "Permission to update blogs"),
            ("blog_schedule", "Permission to schedule blogs"),
            ("blog_view", "Permission to view blogs"),
            ("blog_reject", "Permission to reject blogs"),
            ("blog_create", "Permission to create blogs"),
            ("blog_edit", "Permission to edit blogs"),
            ("blog_delete", "Permission to delete blogs"),
            ("help_request_view", "Permission to view help requests"),
            ("help_request_resolve", "Permission to resolve help requests"),
            ("audit_log_view", "Permission to view audit logs"),
            ("role_create", "Permission to create roles"),
            ("role_view", "Permission to view roles"),
            ("role_assign_permissions", "Permission to assign permissions"),
            ("role_delete", "Permission to delete role"),
            ("role_remove_permissions", "Permission to remove permission from a role"),
            ("auth_enable_2fa", "Permission to enable Two Factor Authentication"),
            ("email_template_create", "Permission to create Email Template"),
            ("email_template_view", "Permission to view Email Template"),
            ("email_template_edit", "Permission to edit Email Template"),
            ("email_template_delete", "Permission to Delete Email Template"),
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
        List<Role> existingRoles = await context.Roles
            .Include(r => r.Permissions)  // Eager-load to check/update permissions
            .ToListAsync(cancellationToken);

        // Define roles and their permissions (unchanged)
        var rolesToSeed = new Dictionary<string, (string Description, List<string> PermissionNames)>
        {
            {
                "Super admin",
                ("System administrator with full access", new List<string>
                {
                    "dashboard_view",
                    "school_create", "school_view", "school_view_profile", "school_delete", "school_edit",
                    "school_approve", "school_subscription_view", "school_subscription_edit",
                    "admin_view", "admin_edit", "admin_invite", "admin_delete",
                    "blog_approve", "blog_view", "blog_create", "blog_edit", "blog_delete", "blog_publish", "blog_reject", "blog_update", "blog_schedule",
                    "help_request_view", "help_request_resolve",
                    "audit_log_view",
                    "role_create", "role_assign_permissions", "role_delete", "role_view", "role_remove_permissions",
                    "auth_enable_2fa",
                    "email_template_create", "email_template_edit", "email_template_delete", "email_template_view"
                })
            },
            {
                "System Admin",
                ("System Admin with limited administrative access", new List<string>
                {
                    "dashboard_view",
                    "admin_view", "admin_edit", "admin_invite", "admin_delete",
                    "audit_log_view",
                    "role_view",
                    "auth_enable_2fa",
                })
            },
            {
                "Business Administrator",
                ("Business Admin", new List<string>
                {
                    "dashboard_view",
                    "admin_view", "admin_edit", "admin_invite", "admin_delete",
                    "audit_log_view",
                    "auth_enable_2fa",
                    "school_approve", "school_subscription_view", "school_subscription_edit",
                })
            },
            {
                "User and Role Manager",
                ("User and Role Manager ", new List<string>
                {
                    "dashboard_view",
                    "admin_view", "admin_edit", "admin_invite", "admin_delete",
                    "audit_log_view",
                    "auth_enable_2fa",
                    "role_create", "role_assign_permissions",
                })
            },
            {
                "Security Officer",
                ("Security Officer", new List<string>
                {
                    "dashboard_view",
                    "audit_log_view",
                    "auth_enable_2fa",
                    "role_create", "role_assign_permissions",
                })
            },
            {
                "Content and Media Manager",
                ("Content and Media Manager", new List<string>
                {
                    "dashboard_view",
                    "auth_enable_2fa",
                    "blog_approve", "blog_view", "blog_create", "blog_edit",
                    "role_create", "role_assign_permissions",
                })
            },
            {
                "Support Manager",
                ("Support Manager", new List<string>
                {
                    "dashboard_view",
                    "help_request_view", "help_request_resolve",
                    "auth_enable_2fa",
                })
            },
            {
                "Reports Manager",
                ("Reports Manager", new List<string>
                {
                    "dashboard_view",
                    "audit_log_view",
                    "help_request_view", "help_request_resolve",
                    "auth_enable_2fa",
                })
            },
        };

        bool changesMade = false;

        foreach (KeyValuePair<string, (string Description, List<string> PermissionNames)> roleData in rolesToSeed)
        {
            string roleName = roleData.Key;
            (string description, List<string> permissionNames) = roleData.Value;

            Role? role = existingRoles.FirstOrDefault(r => r.Name == roleName);

            if (role == null)
            {
                // Create new role
                role = Role.Create(roleName, description, true);
                context.Roles.Add(role);
                logger.LogInformation("Creating new role '{RoleName}'", roleName);
                changesMade = true;
            }
            else
            {
                // Update description if changed
                if (role.Description != description)
                {
                    role.Description = description;
                    changesMade = true;
                }
            }

            // Add missing permissions
            foreach (string permissionName in permissionNames)
            {
                Permission? permission = permissions.FirstOrDefault(p => p.Name == permissionName);
                if (permission == null)
                {
                    logger.LogWarning("Permission '{PermissionName}' not found for role '{RoleName}'", permissionName, roleName);
                    continue;
                }

                if (!role.Permissions.Any(p => p.Name == permissionName))
                {
                    role.AddPermission(permission);
                    changesMade = true;
                }
            }
        }

        if (changesMade)
        {
            await context.SaveChangesAsync(cancellationToken);
            logger.LogInformation("Updated roles and permissions");
        }
        else
        {
            logger.LogInformation("No changes needed for roles");
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

    //private async Task SyncToKeycloak()
    //{
    //    try
    //    {
    //        logger.LogInformation("Starting synchronization with Keycloak");

    //        List<Role> roles = await context.Roles.Include(r => r.Permissions).ToListAsync();
    //        List<Permission> permissions = await context.Permissions.ToListAsync();

    //        bool updated = false;

    //        // Step 1: Sync individual roles to Keycloak
    //        foreach (Role role in roles)
    //        {
    //            Application.Interfaces.KeycloakRoleDto? existingKeycloakRole = await _keycloakRolesService.GetRealmRoleByNameAsync(role.Name);

    //            if (existingKeycloakRole != null)
    //            {
    //                logger.LogInformation("Role '{RoleName}' already exists in Keycloak with ID '{KeycloakId}'.",
    //                    role.Name, existingKeycloakRole.Id);

    //                // Update KeycloakId in local DB if missing or outdated
    //                if (string.IsNullOrWhiteSpace(role.KeycloakId) || role.KeycloakId != existingKeycloakRole.Id)
    //                {
    //                    role.KeycloakId = existingKeycloakRole.Id;
    //                    updated = true;
    //                }

    //                // Optionally update description if changed (assuming UpdateRealmRoleAsync can handle it)
    //                await _keycloakRolesService.UpdateRealmRoleAsync(role.Name, role.Description);
    //            }
    //            else
    //            {
    //                // Create new role
    //                bool created = await _keycloakRolesService.CreateRealmRoleAsync(role.Name, role.Description);
    //                if (created)
    //                {
    //                    logger.LogInformation("Created role '{RoleName}' in Keycloak.", role.Name);

    //                    // Retrieve to get ID
    //                    Application.Interfaces.KeycloakRoleDto? newKeycloakRole = await _keycloakRolesService.GetRealmRoleByNameAsync(role.Name);
    //                    if (newKeycloakRole != null)
    //                    {
    //                        role.KeycloakId = newKeycloakRole.Id;
    //                        updated = true;
    //                    }
    //                }
    //                else
    //                {
    //                    logger.LogWarning("Failed to create role '{RoleName}' in Keycloak.", role.Name);
    //                }
    //            }
    //        }

    //        // Step 2: Sync individual permissions as roles to Keycloak
    //        foreach (Permission permission in permissions)
    //        {
    //            Application.Interfaces.KeycloakRoleDto? existingKeycloakPermission = await _keycloakRolesService.GetRealmRoleByNameAsync(permission.Name);

    //            if (existingKeycloakPermission != null)
    //            {
    //                if (string.IsNullOrWhiteSpace(permission.KeycloakId) || permission.KeycloakId != existingKeycloakPermission.Id)
    //                {
    //                    permission.KeycloakId = existingKeycloakPermission.Id;
    //                    updated = true;
    //                }

    //                // Optionally update description
    //                await _keycloakRolesService.UpdateRealmRoleAsync(permission.Name, permission.Description);
    //            }
    //            else
    //            {
    //                bool created = await _keycloakRolesService.CreateRealmRoleAsync(permission.Name, permission.Description);
    //                if (created)
    //                {
    //                    Application.Interfaces.KeycloakRoleDto? newPermissionRole = await _keycloakRolesService.GetRealmRoleByNameAsync(permission.Name);
    //                    if (newPermissionRole != null)
    //                    {
    //                        permission.KeycloakId = newPermissionRole.Id;
    //                        updated = true;
    //                    }
    //                }
    //                else
    //                {
    //                    logger.LogWarning("Failed to create permission role '{PermissionName}' in Keycloak.", permission.Name);
    //                }
    //            }
    //        }

    //        // Step 3: Sync composite roles (add permissions as composites to each role)
    //        foreach (Role role in roles)
    //        {
    //            if (!role.Permissions.Any())
    //            {
    //                continue;
    //            }

    //            var permissionRoleNames = role.Permissions.Select(p => p.Name).ToList();

    //            // Ensure all permission roles exist (already handled in Step 2, but double-check)
    //            foreach (string permissionName in permissionRoleNames)
    //            {
    //                Application.Interfaces.KeycloakRoleDto? existingPermissionRole = await _keycloakRolesService.GetRealmRoleByNameAsync(permissionName);
    //                if (existingPermissionRole == null)
    //                {
    //                    logger.LogWarning("Permission role '{PermissionName}' not found in Keycloak for composite sync to role '{RoleName}'. Skipping composite add for this permission.", permissionName, role.Name);
    //                    permissionRoleNames.Remove(permissionName);  // Avoid adding non-existent
    //                }
    //            }

    //            if (permissionRoleNames.Any())
    //            {
    //                // Add as composites (this will make the role include these permission roles)
    //                bool compositesAdded = await _keycloakRolesService.AddCompositeRolesAsync(role.Name, permissionRoleNames);
    //                if (compositesAdded)
    //                {
    //                    logger.LogInformation("Added composite permissions to role '{RoleName}' in Keycloak.", role.Name);
    //                }
    //                else
    //                {
    //                    logger.LogWarning("Failed to add composite permissions to role '{RoleName}' in Keycloak.", role.Name);
    //                }
    //            }
    //        }

    //        if (updated)
    //        {
    //            await context.SaveChangesAsync();
    //            logger.LogInformation("Updated KeycloakId for roles and permissions after sync.");
    //        }

    //        logger.LogInformation("Finished synchronization with Keycloak.");
    //    }
    //    catch (Exception ex)
    //    {
    //        logger.LogError(ex, "Failed to synchronize with Keycloak");
    //        // Don't rethrow - we want local seeding to succeed even if Keycloak sync fails
    //    }
    //}


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

