using System;
using Application;
using Application.Dashboard;
using Application.Roles.GetRolesWithPermission;
using Domain.Users;

namespace Web.Api.Endpoints.Roles;

internal sealed class GetRoles : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("roles", async (
                int? page,
                int? pageSize,
                IQueryHandler<GetRolesWithPermissionQuery, PaginatedList<RoleDto>> handler,
                CancellationToken cancellationToken) =>
            {
                var query = new GetRolesWithPermissionQuery(page, pageSize);

                Result<PaginatedList<RoleDto>> result = await handler.Handle(query, cancellationToken);

                return result.Match(Results.Ok, CustomResults.Problem);
            })
            .WithTags(Tags.Roles)
            .Produces<PaginatedList<RoleDto>>(StatusCodes.Status200OK)
        .RequireAuthorization();
    }
}
