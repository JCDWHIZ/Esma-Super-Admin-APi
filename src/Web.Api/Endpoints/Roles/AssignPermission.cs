using System;
using Application.Dashboard;
using Application.Roles.AssignPermissionToRole;
using Infrastructure.Authorization;

namespace Web.Api.Endpoints.Roles;

internal sealed class AssignPermission : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("roles/{publicId:guid}/permissions/{permissionId:guid}", async (
                Guid publicId,
                Guid permissionId,
                ICommandHandler<AssignPermissionToRoleCommand, string> handler,
                CancellationToken cancellationToken) =>
            {
                var command = new AssignPermissionToRoleCommand(publicId, permissionId);

                Result<string> result = await handler.Handle(command, cancellationToken);

                return result.Match(Results.Ok, CustomResults.Problem);
            })
            .RequireAuthorization()
            .WithAudit("A permission was assigned to a role")
            .Produces<string>(StatusCodes.Status200OK)
            .RequireAuthorization(new RequirePermissionAttribute("AssignPermissions"));
    }
}
