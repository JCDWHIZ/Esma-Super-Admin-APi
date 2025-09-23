using System;
using Application;
using Application.Dashboard;
using Application.Roles.GetRolesWithPermission;
using Domain.Users;
using Infrastructure.Authorization;

namespace Web.Api.Endpoints.Roles;

internal sealed class GetRoles : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("roles", async (
                IQueryHandler<GetRolesWithPermissionQuery, List<RoleDto>> handler,
                CancellationToken cancellationToken) =>
            {
                var query = new GetRolesWithPermissionQuery();

                Result<List<RoleDto>> result = await handler.Handle(query, cancellationToken);

                return result.Match(Results.Ok, CustomResults.Problem);
            })
            .WithTags(Tags.Roles)
            .Produces<PaginatedList<RoleDto>>(StatusCodes.Status200OK)
            .RequireAuthorization(new RequirePermissionAttribute("ViewRoles"));
    }
}
