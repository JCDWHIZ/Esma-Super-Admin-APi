using Application.Roles.GetRolesWithPermission;
using Domain.Users;
using Infrastructure.Authorization;

namespace Web.Api.Endpoints.Roles;

internal sealed class GetRoles : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("roles", async (
                IQueryHandler<GetRolesWithPermissionQuery, List<RolesDto>> handler,
                CancellationToken cancellationToken) =>
            {
                var query = new GetRolesWithPermissionQuery();

                Result<List<RolesDto>> result = await handler.Handle(query, cancellationToken);

                return result.Match(Results.Ok, CustomResults.Problem);
            })
            .WithTags(Tags.Roles)
            .Produces<List<RolesDto>>(StatusCodes.Status200OK)
            .RequireAuthorization(new RequirePermissionAttribute("role_view"));
    }
}
