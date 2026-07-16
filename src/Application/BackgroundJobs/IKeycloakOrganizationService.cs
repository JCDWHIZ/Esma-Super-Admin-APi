using Application.Abstractions.Authentication;
using Application.Abstractions.Models;
using Application.Interfaces;
using Application.School;
using Application.School.CreateSchool;
using Domain.Schools;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using IApplicationDbContext = Application.Abstractions.Data.IApplicationDbContext;

namespace Application.BackgroundJobs;

public interface IKeycloakOrganizationService
{
    Task SendSchoolCreateTenantMessageAsync(int schoolId, CancellationToken cancellationToken);
    Task CreateAdmin(int userId, CancellationToken cancellationToken);
    Task EditAdmin(int userId, CancellationToken cancellationToken);
    Task DeleteAdmin(int userId, CancellationToken cancellationToken);
    Task<string> CreateSchoolAdmin(int userId, int schoolId, CancellationToken cancellationToken);
    Task<string> CreateKeycloackSchool(Schools school, CancellationToken cancellationToken);
}

public class KeycloakOrganizationService : IKeycloakOrganizationService
{
    private readonly IApplicationDbContext _dbContext;
    private readonly ITokenProvider _tokenProvider;
    private readonly IEmailService _emailService;
    private readonly IKeycloakService _keycloakService;
    private readonly IMessageProducer _messageProducer;
    private readonly IConfiguration _configuration;
    private readonly IKeycloakRolesService _keycloakRolesService;
    private readonly ILogger<KeycloakOrganizationService> _logger;

    public KeycloakOrganizationService(IApplicationDbContext dbContext, IKeycloakService keycloakService, IConfiguration configuration, IMessageProducer messageProducer, ILogger<KeycloakOrganizationService> logger, ITokenProvider tokenProvider, IEmailService emailService, IKeycloakRolesService keycloakRolesService)
    {
        _dbContext = dbContext;
        _keycloakService = keycloakService;
        _configuration = configuration;
        _messageProducer = messageProducer;
        _logger = logger;
        _emailService = emailService;
        _tokenProvider = tokenProvider;
        _keycloakRolesService = keycloakRolesService;
    }
    public async Task CreateAdmin(int userId, CancellationToken cancellationToken)
    {

        User? user = await _dbContext.Users
          .Include(u => u.Role) // Include Role to access its properties
          .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
        if (user == null)
        {
            return;
        }

        try
        {
            var inviteRequest = new InviteUserRequestDto
            {
                Username = user.Email,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                EmailVerified = true,
                Enabled = true,
                Attributes = new()
                {
                    { "internal_user_id", new() { user.PublicId.ToString() } },
                    {"internal_user_role", new() { user.Role.Name } }
                },
                RequiredActions = new List<string>
                {
                    "UPDATE_PASSWORD"
                }
            };

            // keycloaks sends email
            // might want to disable that for now 
            // use emailService to send email instead, just use keycloak to create user
            // await _keycloakService.InviteUserAsync(inviteRequest);
            // 1. Create the user in Keycloak
            string keycloakUserId = await _keycloakService.CreateUserAsync(inviteRequest);
            // 2. Add the user to organization
            await _keycloakService.AddUserToOrganizationAsync(keycloakUserId);
            if (!Guid.TryParse(keycloakUserId, out Guid keycloakId))
            {
                throw new InvalidOperationException("Returned Keycloak ID is not a valid GUID.");
            }
            user.KeycloakUserId = keycloakId;
            await _dbContext.SaveChangesAsync(cancellationToken);
            await _keycloakRolesService.AssignRoleToUserAsync(keycloakUserId, user.Role.Name);

            string resetToken = _tokenProvider.CreateOnboardingToken(user);
            var emailMessage = new EmailMessage
            {
                Email = user.Email,
                Title = "Set Up Your Account",
                Name = $"{user.FirstName} {user.LastName}",
                Description = "You've been invited to join our platform. To get started, please click the button below to set your password and activate your account. This link is secure and will expire after a period of time for your protection.",
                EmailButton = true,
                ButtonLink = $"{_configuration["Frontend:BaseUrl"]}/auth/password/set-password?token={resetToken}",
                ButtonText = "Set Your Password"
            };

            await _emailService.SendEmailAsync(emailMessage);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while creating admin in Keycloak");
        }
    }

    public async Task<string> CreateSchoolAdmin(int userId, int schoolId, CancellationToken cancellationToken)
    {

        Domain.Schools.Schools? school = await _dbContext.Schools.Include(s => s.User).FirstOrDefaultAsync(s => s.Id == schoolId, cancellationToken);
        if (school == null)
        {
            return string.Empty;
        }

        try
        {
            var inviteRequest = new InviteUserRequestDto
            {
                Username = school.User.Email,
                Email = school.User.Email,
                FirstName = school.User.FirstName,
                LastName = school.User.LastName,
                EmailVerified = true,
                Enabled = true,
                //Attributes = new()
                //{
                //    { "internal_user_id", new() { school.User.PublicId.ToString() } },
                //    {"internal_user_role", new() { school.User.Role.ToString() } }
                //},
            };
            string keycloakUserId = await _keycloakService.CreateUserAsync(inviteRequest);
            await _keycloakService.AddUserToOrganizationAsync(keycloakUserId, school.OrganizationId);
            //if (!Guid.TryParse(keycloakUserId, out Guid keycloakId))
            //{
            //    throw new InvalidOperationException("Returned Keycloak ID is not a valid GUID.");
            //}
            SchoolAdmins? user = await _dbContext.SchoolAdmins.FirstOrDefaultAsync(s => s.Id == school.User.Id, cancellationToken) ?? throw new InvalidOperationException($"School admin with ID {school.User.Id} not found.");
            user.KeycloakUserId = keycloakUserId;
            await _dbContext.SaveChangesAsync(cancellationToken);
            return keycloakUserId ?? string.Empty;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while creating school admin in Keycloak");
            return string.Empty;
        }
    }

    public async Task EditAdmin(int userId, CancellationToken cancellationToken)
    {
        User? user = await _dbContext.Users
          .Include(u => u.Role)
          .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
        if (user == null || user.KeycloakUserId == Guid.Empty)
        {
            return;
        }

        var updateRequest = new UpdateUserRequestDto
        {
            Username = user.Email,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Enabled = true,
            Attributes = new()
            {
                { "internal_user_id", new() { user.PublicId.ToString() } },
                { "internal_user_role", new() { user.Role.Name } }
            }
        };

        try
        {
            await _keycloakService.UpdateUserAsync(user.KeycloakUserId.ToString()!, updateRequest);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update admin in Keycloak");
        }
    }

    public async Task DeleteAdmin(int userId, CancellationToken cancellationToken)
    {
        User? user = await _dbContext.Users.FindAsync([userId], cancellationToken: cancellationToken);
        if (user == null || user.KeycloakUserId == Guid.Empty)
        {
            return;
        }

        try
        {
            await _keycloakService.DeleteUserAsync(user.KeycloakUserId.ToString()!);

            // optionally also remove from DB
            _dbContext.Users.Remove(user);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete admin in Keycloak");
        }
    }

    public async Task SendSchoolCreateTenantMessageAsync(int schoolId, CancellationToken cancellationToken)
    {
        Domain.Schools.Schools? school = await _dbContext.Schools
            .Include(s => s.User)
            .Include(s => s.Subscriptions)
            .Include(s => s.Modules)
            .FirstOrDefaultAsync(s => s.Id == schoolId, cancellationToken);
        if (school == null)
        {
            return;
        }

        try
        {
            CreateTenantMessage tenantMessage = TenantMessageMapper.BuildCreateTenantMessage(school);

            await _messageProducer.SendMessageAsync(
                "CreateTenant",
                tenantMessage,
                _configuration["Kafka:CreateTenantTopic"]);

            _logger.LogInformation("tenant creation task enqueued for school: {SchoolId}",
                school.Id);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException(
                $"CreateOrganizationForSchoolAsync failed for school {schoolId}",
                ex);
        }
    }

    public async Task<string> CreateKeycloackSchool(Schools school, CancellationToken cancellationToken)
    {
        try
        {
            string organizationId = await _keycloakService.CreateOrganizationAsync(school.SchoolName);
            school.OrganizationId = organizationId;
            string keycloakcUserId = await CreateSchoolAdmin(school.User.Id, school.Id, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Organization created for school: {SchoolId}",
                school.Id);
            return keycloakcUserId;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException(
                $"CreateOrganizationForSchoolAsync failed for school {school.Id}",
                ex);
        }
    }
}



