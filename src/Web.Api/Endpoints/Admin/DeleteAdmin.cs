using Application.Admin.DeleteAdmin;

namespace Web.Api.Endpoints.Admin;

internal sealed class DeleteAdminEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("/admins/{publicId}", async (
            Guid publicId,
            ICommandHandler<DeleteAdminCommand, string> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new DeleteAdminCommand(publicId);

            Result<string> result = await handler.Handle(command, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.Admin)
        .Produces<string>(StatusCodes.Status200OK)
        .WithAudit("An admin was deleted")
        .RequireAuthorization();
    }
}
