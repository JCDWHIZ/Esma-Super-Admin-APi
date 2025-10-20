using Application.Auth.ChangePassword;

namespace Web.Api.Endpoints.Auth;

internal sealed class ChangePasswordEndpoint : IEndpoint
{
    public sealed class Request
    {
        public string CurrentPassword { get; init; }
        public string NewPassword { get; init; }
    }

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/auth/change-password", async (
            Request request,
            ICommandHandler<ChangePasswordCommand, string> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new ChangePasswordCommand(
                request.NewPassword);

            Result<string> result = await handler.Handle(command, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.Auth)
        .RequireAuthorization();
    }
}
