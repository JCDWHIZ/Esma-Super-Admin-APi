using System;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Application.Interfaces;
using Application.BackgroundJobs;
using Hangfire;
using Microsoft.Extensions.Configuration;
using Domain.Users;
using IApplicationDbContext = Application.Abstractions.Data.IApplicationDbContext;
using Domain.Roles;


// add softdelete and restore for admin endpoint
// add status for user so that super admin can approve 
// add bulk approval for approve school change publicId to array of string ids
namespace Application.Admin.InviteAdmin;

public sealed class InviteAdminCommandHandler(IApplicationDbContext _context) : ICommandHandler<InviteAdminCommand, UserDto>
{
    async Task<Result<UserDto>> ICommandHandler<InviteAdminCommand, UserDto>.Handle(InviteAdminCommand command, CancellationToken cancellationToken)
    {
        if (await _context.Users.AnyAsync(u => u.Email == command.Email, cancellationToken))
        {
            return Result.Failure<UserDto>(UserErrors.AlreadyExists());
        }

        Domain.Roles.Role? role = await _context.Roles.FirstOrDefaultAsync(r => r.Name == command.RoleName, cancellationToken);
        if (role == null)
        {
            return Result.Failure<UserDto>(RoleErrors.NotFound());
        }

        var newUser = new User
        {
            Email = command.Email,
            RoleId = role.Id, // Assign RoleId instead of Role enum
            Username = command.Email,
            FirstName = command.FirstName,
            LastName = command.LastName,
            ProfilePic = command.ProfilePic ?? "string",
            PhoneNumber = command.PhoneNumber
        };

        _context.Users.Add(newUser);
        await _context.SaveChangesAsync(cancellationToken);

        BackgroundJob.Enqueue<IKeycloakOrganizationService>(
            service => service.CreateAdmin(newUser.Id, cancellationToken));

        return new UserDto
        {
            PublicId = newUser.PublicId,
            Email = newUser.Email,
            Username = newUser.Username,
            FirstName = newUser.FirstName,
            LastName = newUser.LastName,
            PhoneNumber = newUser.PhoneNumber,
            ProfilePic = newUser.ProfilePic
        };
    }
}

// public class InviteAdmincommandHandler(IApplicationDbContext context, IEmailService emailService, HttpClient httpClient, IConfiguration configuration) : ICommandHandler<InviteAdminCommand, bool>
// {
//     private readonly IApplicationDbContext _context = context;
//     private readonly IEmailService _emailService = emailService;
//     private readonly HttpClient _httpClient = httpClient;
//     private readonly IConfiguration _configuration = configuration;

//     async Task<bool> ICommandHandler<InviteAdminCommand, bool>.Handle(InviteAdminCommand command, CancellationToken cancellationToken)
// {
//     var adminToken = await GetAdminTokenAsync();
//     if(adminToken != null){
//         _httpClient.DefaultcommandHeaders.Authorization = new AuthenticationHeaderValue("Bearer", adminToken);
//         using var formData = new MultipartFormDataContent
//     {
//         { new StringContent(command.Email), "email" },
//         { new StringContent(command.FirstName), "firstName" },
//         { new StringContent(command.LastName), "lastName" }
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


// public async Task<UserDto> Handle(InviteAdminCommand command, CancellationToken cancellationToken)
// {
// var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == command.Email, cancellationToken);
// if (existingUser != null)
// {
//     throw new InvalidOperationException("User with this email already exists.");
// }

// var newUser = new User
// {
//     Email = command.Email,
//     Role = command.Role,
//     FirstName = command.FirstName,
//     LastName = command.LastName,
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
