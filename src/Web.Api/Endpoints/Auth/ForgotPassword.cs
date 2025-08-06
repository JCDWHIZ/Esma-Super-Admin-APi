// using System;
// using Application.Auth.ForgotPassword;

// namespace Web.Api.Endpoints.Auth;

// internal sealed class ForgotPassword : IEndpoint
// {
//     public sealed class Request
//     {
//         public string Email { get; init; } = default!;
//     }

//     public void MapEndpoint(IEndpointRouteBuilder app)
//     {
//         app.MapPost("auth/forgot-password", async (
//             Request request,
//             ICommandHandler<ForgotPasswordCommand, ForgotPasswordResponseDto> handler,
//             CancellationToken cancellationToken) =>
//         {
//             var command = new ForgotPasswordCommand(request.Email);

//             Result<ForgotPasswordResponseDto> result = await handler.Handle(command, cancellationToken);

//             return result.Match(Results.Ok, CustomResults.Problem);
//         })
//         .WithTags(Tags.Auth);
//     }
// }