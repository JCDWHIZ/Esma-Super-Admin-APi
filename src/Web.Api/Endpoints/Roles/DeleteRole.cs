using Application.Roles.DeleteRole;
using Infrastructure.Authorization;

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
            .RequireAuthorization(new RequirePermissionAttribute("role_delete"))
            .WithAudit("A role was deleted")
            .Produces<string>(StatusCodes.Status200OK)
            .WithTags(Tags.Roles);
    }
}
