using Application.Admin;
using Application.Admin.GetUserProfile;
using Web.Api.Extensions;

namespace Web.Api.Endpoints.Admin;

internal sealed class GetUserProfileEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/admin/profile", async (
            IQueryHandler<GetUserProfileQuery, UserDto> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetUserProfileQuery();

            Result<UserDto> result = await handler.Handle(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.Admin)
        .RequireAuthorization();
    }
}
