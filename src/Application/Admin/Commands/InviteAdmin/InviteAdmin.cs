using System;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using admin_service.Application.Admin.Queries;
using admin_service.Application.Common.Exceptions;
using admin_service.Application.Common.Interfaces;
using admin_service.Domain.Entities;
using admin_service.Domain.Enums;
using Hangfire;
using Microsoft.Extensions.Configuration;



namespace admin_service.Application.Admin.Commands.InviteAdmin;

public class InviteAdminValidator : AbstractValidator<InviteUserCommand>
{
    public InviteAdminValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email format.");

        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required.");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required.");

        RuleFor(x => x.Role)
            .IsInEnum().WithMessage("Invalid role.");
    }
};



public class InviteUserCommand : IRequest<UserDto>
{
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public Roles Role { get; set; }
}

public class InviteUserCommandHandler : IRequestHandler<InviteUserCommand, UserDto>
{
    private readonly IApplicationDbContext _context;

    private readonly IMapper _mapper;

    public InviteUserCommandHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;

    }

    public async Task<UserDto> Handle(InviteUserCommand request, CancellationToken cancellationToken)
    {
        var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email, cancellationToken);

        if (existingUser != null)
        {
            throw new AlreadyExistsException("User with this email already exists.");
        }

        var newUser = new User
        {
            Email = request.Email,
            Role = request.Role,
            Username = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
        };

        _context.Users.Add(newUser);
        await _context.SaveChangesAsync(cancellationToken);

        BackgroundJob.Enqueue<IKeycloakOrganizationService>(
        service => service.CreateAdmin(newUser.Id));

        return new UserDto
        {
            Id = newUser.Id,
            Email = newUser.Email,
            Role = newUser.Role,
            FirstName = newUser.FirstName,
            LastName = newUser.LastName,
        };
    }
}

// public class InviteAdminRequestHandler(IApplicationDbContext context, IEmailService emailService, HttpClient httpClient, IConfiguration configuration) : IRequestHandler<InviteUserCommand, bool>
// {
//     private readonly IApplicationDbContext _context = context;
//     private readonly IEmailService _emailService = emailService;
//     private readonly HttpClient _httpClient = httpClient;
//     private readonly IConfiguration _configuration = configuration;

//     async Task<bool> IRequestHandler<InviteUserCommand, bool>.Handle(InviteUserCommand request, CancellationToken cancellationToken)
// {
//     var adminToken = await GetAdminTokenAsync();
//     if(adminToken != null){
//         _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", adminToken);
//         using var formData = new MultipartFormDataContent
//     {
//         { new StringContent(request.Email), "email" },
//         { new StringContent(request.FirstName), "firstName" },
//         { new StringContent(request.LastName), "lastName" }
//     };

//     var keycloakUrl = $"{_configuration["Keycloak:BaseUrl"]}/admin/realms/{_configuration["Keycloak:Realm"]}/organizations/8de4abf4-46b6-4e48-82e8-ad0ed8e5e797/members/invite-user";

//     var response = await _httpClient.PostAsync(keycloakUrl, formData);
//     var responseText = await response.Content.ReadAsStringAsync(cancellationToken);

//     return response.IsSuccessStatusCode;
//     }else {
//         Console.WriteLine("no token");
//         return false;

//     }
// }


//     private async Task<string?> GetAdminTokenAsync()
// {
//     try
//     {
//         var tokenEndpoint = $"{_configuration["Keycloak:BaseUrl"]}/realms/{_configuration["Keycloak:Realm"]}/protocol/openid-connect/token";

//         var clientId = _configuration["Keycloak:clientId"]!;
//         var clientSecret = _configuration["Keycloak:ClientSecret"]!;
//         var AdminUsername = _configuration["Keycloak:AdminUsername"]!;
//         var AdminPassword = _configuration["Keycloak:AdminPassword"]!;

//         var parameters = new Dictionary<string, string>
//         {
//             { "client_id", clientId },
//             { "username", AdminUsername },
//             { "password", AdminPassword },
//             { "client_secret", clientSecret },
//             { "grant_type", "password" }
//         };

//         var response = await _httpClient.PostAsync(tokenEndpoint, new FormUrlEncodedContent(parameters));
//         var responseBody = await response.Content.ReadAsStringAsync();

//         if(response.IsSuccessStatusCode)
//         {
//             var tokenResponse = JsonSerializer.Deserialize<TokenResponse>(responseBody);
//             return tokenResponse?.AccessToken;
//         }
//         return null;
//     }
//     catch (Exception ex)
//     {
//         Console.WriteLine($"Error getting token: {ex.Message}");
//         return null;
//     }
// }
//     }


// public async Task<UserDto> Handle(InviteUserCommand request, CancellationToken cancellationToken)
// {
// var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email, cancellationToken);
// if (existingUser != null)
// {
//     throw new InvalidOperationException("User with this email already exists.");
// }

// var newUser = new User
// {
//     Email = request.Email,
//     Role = request.Role,
//     FirstName = request.FirstName,
//     LastName = request.LastName,
// };

// _context.Users.Add(newUser);
// await _context.SaveChangesAsync(cancellationToken);
// // var inviteToken = Guid.NewGuid().ToString();
// // var inviteLink = $"https://yourapp.com/invite?token={inviteToken}";
// // await _emailService.SendEmailAsync(newUser.Email, "Admin Invitation",
// //     $"You have been invited as an admin. Click <a href='{inviteLink}'>here</a> to accept.");
// return new UserDto
// {
//     Email = newUser.Email,
//     Role = newUser.Role
// };


// }