using System;
using Application.Auth.SetPassword;
using Application.Interfaces;

namespace Web.Api.Endpoints.Auth;

internal sealed class SetPasswordEndpoint : IEndpoint
{
    public sealed class Request
    {
        public string NewPassword { get; init; }
        public string AccessToken { get; init; }
    }

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("auth/change-password", async (
            Request request,
            ICommandHandler<SetPasswordCommand, string> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new SetPasswordCommand(
                request.NewPassword,
                request.AccessToken
            );

            Result<string> result = await handler.Handle(command, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.Auth);
    }
}