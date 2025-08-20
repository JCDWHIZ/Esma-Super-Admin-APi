using System;
using Application.Roles.AssignPermissionToRole;

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
            .WithTags(Tags.Roles);
    }
}
