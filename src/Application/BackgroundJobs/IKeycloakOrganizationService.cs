using Application.Abstractions.Authentication;
using Application.Abstractions.Models;
using Application.Interfaces;
using Application.School;
using Application.School.CreateSchool;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using IApplicationDbContext = Application.Abstractions.Data.IApplicationDbContext;

namespace Application.BackgroundJobs;

public interface IKeycloakOrganizationService
{
    Task CreateOrganizationForSchoolAsync(int schoolId, CancellationToken cancellationToken);
    Task CreateAdmin(int userId, CancellationToken cancellationToken);
    Task EditAdmin(int userId, CancellationToken cancellationToken);
    Task DeleteAdmin(int userId, CancellationToken cancellationToken);
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

    public async Task CreateSchoolAdmin(int userId, int schoolId, CancellationToken cancellationToken)
    {

        Domain.Schools.Schools? school = await _dbContext.Schools.Include(s => s.User).FirstOrDefaultAsync(s => s.Id == schoolId, cancellationToken);
        if (school == null)
        {
            return;
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
                //    { "internal_user_id", new() { user.PublicId.ToString() } },
                //    {"internal_user_role", new() { user.Role.ToString() } }
                //},
            };
            string keycloakUserId = await _keycloakService.CreateUserAsync(inviteRequest);
            await _keycloakService.AddUserToOrganizationAsync(keycloakUserId, school.OrganizationId);
            //if (!Guid.TryParse(keycloakUserId, out Guid keycloakId))
            //{
            //    throw new InvalidOperationException("Returned Keycloak ID is not a valid GUID.");
            //}

            //user.KeycloakUserId = keycloakId;
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while creating school admin in Keycloak");
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

    public async Task CreateOrganizationForSchoolAsync(int schoolId, CancellationToken cancellationToken)
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
            string organizationId = await _keycloakService.CreateOrganizationAsync(school.SchoolName);
            school.OrganizationId = organizationId;
            await _dbContext.SaveChangesAsync(cancellationToken);
            await CreateSchoolAdmin(school.User.Id, schoolId, cancellationToken);
            //     var payload = new Dictionary<string, object>
            // {
            //     { "schoolId", school.Id },
            //     { "schoolPublicId", school.PublicId },
            //     { "organizationId", organizationId },
            //     { "schoolName", school.SchoolName },
            //     { "email", school.EmailAddress },
            //     { "firstName", school.User.FirstName },
            //     { "lastName",  school.User.LastName },
            //     { "role",      school.User.Role.ToString() },
            //     { "username",  school.User.Username },
            //     { "phoneNumber", school.User?.PhoneNumber ?? string.Empty }
            // };

            //     string token = _tokenService.GenerateToken(payload);
            //     var message = new EmailMessage
            //     {
            //         Email = school.EmailAddress,
            //         Title = "Your School Organization is Ready",
            //         SchoolName = school.SchoolName,
            //         Description = "We've successfully onboarded your school to our platform. We’re excited to share that your school has been successfully added to our platform! This marks the beginning of a seamless, integrated experience designed to empower your institution with the tools and support needed to thrive. Welcome aboard—we’re looking forward to growing with you.",
            //         EmailButton = true,
            //         ButtonLink = $"{_configuration["Frontend:BaseUrl"]}/onboarding?token={token}",
            //         ButtonText = "Complete Your Setup"
            //     };
            //     await _emailService.SendEmailAsync(message);

            // send message to kafak for tenant service creation
            CreateTenantMessage tenantMessage = TenantMessageMapper.BuildCreateTenantMessage(school);

            await _messageProducer.SendMessageAsync(
                "CreateTenant",
                tenantMessage,
                _configuration["Kafka:CreateTenantTopic"]);

            _logger.LogInformation("Organization created and tenant creation task enqueued for school: {SchoolId}",
                school.Id);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException(
                $"CreateOrganizationForSchoolAsync failed for school {schoolId}",
                ex);
        }
    }
}



