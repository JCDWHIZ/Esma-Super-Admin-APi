// using admin_service.Application.Authentication.Commands.LoginCommand;
// using admin_service.Application.Authentication.Commands.LogoutCommand;
// using admin_service.Application.Authentication.Commands.Queries;
// using admin_service.Application.Authentication.Commands.RefreshCommand;
// using admin_service.Application.Common.Exceptions;
// using admin_service.Application.Common.Interfaces;
// using Application.Abstractions.Models;
// using admin_service.Infrastructure.Identity;
// using Microsoft.AspNetCore.Http.HttpResults;
// using Microsoft.AspNetCore.Mvc;

// namespace admin_service.Web.Endpoints;

// public class Users : EndpointGroupBase
// {
//     public override void Map(WebApplication app)
//     {
//         app.MapGroup(this)
//             .MapPost( Refresh, "refresh")
//             .MapPost(Login, "login" )
//             .MapPost(Logout, "logout");
//             // .MapIdentityApi<ApplicationUser>();
//     }

//      public async Task<Results<Ok<AuthResult>, BadRequest<AuthResult>>> Login(
//     ISender sender,
//     [FromBody] LoginDto loginDto)
//     {
//         var loginCommand = new LoginCommand(loginDto.Email, loginDto.Password);
//         var authResult = await sender.Send(loginCommand);

//         if (!authResult.Succeeded)
//         {
//             return TypedResults.BadRequest(authResult);
//         }
//         return TypedResults.Ok(authResult);
//     }
//      public async Task<Results<Ok<AuthResult>, BadRequest<AuthResult>>> Refresh(
//     ISender sender,
//     [FromBody] string refreshToken)
//     {
//         var refreshCommand = new RefreshTokenCommand(refreshToken);
//         var authResult = await sender.Send(refreshCommand);

//         if (!authResult.Succeeded)
//         {
//             return TypedResults.BadRequest(authResult);
//         }
//         return TypedResults.Ok(authResult);
//     }

//     public async Task<Results<Ok, BadRequest>> Logout(ISender sender, [FromBody] string refreshToken)
//     {
//         await sender.Send(new LogoutCommand(refreshToken));
//         return TypedResults.Ok();
//     }



// }



// using admin_service.Application.UseCases;
// using Keycloak.Net.Models.Users;

using Application.Admin.InviteAdmin;
using admin_service.Application.Admin.Queries;
using Application.Abstractions.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

public class Users : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
        // .RequireAuthorization()
        .MapPost(InviteUser, "invite/admin")
        .MapGet(GetAllAdminWithPagination, "/");
    }

    public async Task<Ok<UserDto>> InviteUser([FromBody] InviteUserCommand request, ISender sender)
    {
        var result = await sender.Send(request);
        return TypedResults.Ok(result);
    }

    public async Task<Ok<PaginatedList<UserDto>>> GetAllAdminWithPagination(ISender sender,
    [AsParameters] GetAdminWithPaginationQuery query)
    {

        var result = await sender.Send(query);
        return TypedResults.Ok(result);
    }

}
// private async Task<IResult> SendResetEmail(string email)    
// {
//     if (_sendResetEmailUseCase == null) throw new InvalidOperationException("Dependency not initialized.");
//     var result = await _sendResetEmailUseCase.Execute(email);
//     return result ? Results.Ok("Reset email sent successfully") : Results.BadRequest("Failed to send reset email");
// }




