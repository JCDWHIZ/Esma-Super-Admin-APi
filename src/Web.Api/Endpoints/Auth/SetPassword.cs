// using System;
// using Application.Auth.ChangePassword;
// using Application.Interfaces;

// namespace Web.Api.Endpoints.Auth;

// internal sealed class ChangePasswordEndpoint : IEndpoint
// {
//     public sealed class Request
//     {
//         public string CurrentPassword { get; init; }
//         public string NewPassword { get; init; }
//         public string AccessToken { get; init; }
//     }

//     public void MapEndpoint(IEndpointRouteBuilder app)
//     {
//         app.MapPost("auth/change-password", async (
//             Request request,
//             ICommandHandler<ChangePasswordCommand, ChangePasswordResponseDto> handler,
//             CancellationToken cancellationToken) =>
//         {
//             var command = new ChangePasswordCommand(
//                 request.CurrentPassword,
//                 request.NewPassword,
//                 request.AccessToken
//             );

//             Result<ChangePasswordResponseDto> result = await handler.Handle(command, cancellationToken);

//             return result.Match(Results.Ok, CustomResults.Problem);
//         })
//         .WithTags(Tags.Auth);
//     }
// }