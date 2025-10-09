using Application;
using Application.Roles.GetPermissions;
using Domain.Users;
using Infrastructure.Authorization;
using static Application.Roles.GetPermissions.GetPermissionQueryHandler;
namespace Web.Api.Endpoints.Roles;

internal sealed class GetPermissionsEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/permissions", async (
            IQueryHandler<GetPermissionsQuery, List<GroupedPermissionDto>> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetPermissionsQuery();

            Result<List<GroupedPermissionDto>> result = await handler.Handle(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.Roles)
        .Produces<List<PermissionDto>>(StatusCodes.Status200OK)
        .RequireAuthorization(new RequirePermissionAttribute("role_view"));
    }
}
