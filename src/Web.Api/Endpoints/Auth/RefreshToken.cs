using Application.Auth.Login;
using Application.Auth.RefreshToken;

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
            ICommandHandler<RefreshTokenCommand, LoginCommandResponseDto> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new RefreshTokenCommand(request.RefreshToken);

            Result<LoginCommandResponseDto> result = await handler.Handle(command, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.Auth)
        .Produces<LoginCommandResponseDto>(StatusCodes.Status200OK);
    }
}
