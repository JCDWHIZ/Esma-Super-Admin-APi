using System;
using Application.Auth.RefreshToken;
using Application.Interfaces;

namespace Web.Api.Endpoints.Auth;

internal sealed class RefreshTokenEndpoint : IEndpoint
{
    public sealed class Request
    {
        public string RefreshToken { get; init; } = default!;
    }

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("auth/refresh-token", async (
            Request request,
            ICommandHandler<RefreshTokenCommand, RefreshTokenResponseDto> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new RefreshTokenCommand(request.RefreshToken);

            Result<RefreshTokenResponseDto> result = await handler.Handle(command, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.Auth)
        .Produces<RefreshTokenResponseDto>(StatusCodes.Status200OK);
    }
}
