using Application.Auth.Login;
using Web.Api.Attributes;

namespace Web.Api.Endpoints.Auth;

internal sealed class LoginEndpoint : IEndpoint
{
    public sealed class Request
    {
        public string Email { get; init; } = default!;
        public string Password { get; init; } = default!;
    }
    [Audit("Created Draft Blog on endpoint")]
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("auth/login", async (
            Request request,
            ICommandHandler<LoginCommand, LoginCommandResponseDto> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new LoginCommand(
                Email: request.Email,
                Password: request.Password
            );

            Result<LoginCommandResponseDto> result = await handler.Handle(command, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.Auth)
        .Produces<LoginCommandResponseDto>(StatusCodes.Status200OK);
    }
}
