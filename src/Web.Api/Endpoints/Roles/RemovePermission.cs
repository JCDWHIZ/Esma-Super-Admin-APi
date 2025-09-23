using System;
using Application.Dashboard;
using Application.Roles.RemovePermissionFromRole;
using Infrastructure.Authorization;

namespace Web.Api.Endpoints.Roles;

internal sealed class RemovePermission : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("roles/{publicId:guid}/permissions/{permissionId:guid}", async (
                Guid publicId,
                Guid permissionId,
                ICommandHandler<RemovePermissionCommand, string> handler,
                CancellationToken cancellationToken) =>
            {
                var command = new RemovePermissionCommand(publicId, permissionId);

                Result<string> result = await handler.Handle(command, cancellationToken);

                return result.Match(Results.Ok, CustomResults.Problem);
            })
            .RequireAuthorization(new RequirePermissionAttribute("RemovePermissions"))
            .WithAudit("A permssion was removed from a role")
            .Produces<string>(StatusCodes.Status200OK)
            .WithTags(Tags.Roles);
    }
}
