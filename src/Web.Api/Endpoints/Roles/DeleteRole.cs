using System;
using Application.Dashboard;
using Application.Roles.DeleteRole;

namespace Web.Api.Endpoints.Roles;

internal sealed class DeleteRole : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("roles/{publicId:guid}", async (
                Guid publicId,
                ICommandHandler<DeleteRoleCommand, string> handler,
                CancellationToken cancellationToken) =>
            {
                var command = new DeleteRoleCommand(publicId);

                Result<string> result = await handler.Handle(command, cancellationToken);

                return result.Match(Results.Ok, CustomResults.Problem);
            })
            .RequireAuthorization()
            .WithAudit("A role was deleted")
            .Produces<string>(StatusCodes.Status200OK)
            .WithTags(Tags.Roles);
    }
}
