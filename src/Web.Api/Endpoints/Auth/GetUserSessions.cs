using Application.Auth;
using Web.Api.Extensions;

namespace Web.Api.Endpoints.Auth;

internal sealed class GetUserSessionEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/auth/session", async (
            IQueryHandler<GetUserSessionQuery, GetSessionsCommandResponseDto> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetUserSessionQuery();

            Result<GetSessionsCommandResponseDto> result = await handler.Handle(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.Auth)
        .RequireAuthorization();
    }
}
