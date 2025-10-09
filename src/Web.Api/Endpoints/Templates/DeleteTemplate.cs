using Application.Dashboard;
using Application.Templates.DeleteTemplate;
using Infrastructure.Authorization;

namespace Web.Api.Endpoints.Templates;

internal sealed class DeleteTemplateEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("/templates/{publicId}", async (
            Guid publicId,
            ICommandHandler<DeleteTemplateCommand, string> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new DeleteTemplateCommand(publicId);

            Result<string> result = await handler.Handle(command, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.Templates)
        .WithAudit("A template was deleted")
        .Produces<string>(StatusCodes.Status200OK)
        .RequireAuthorization(new RequirePermissionAttribute("email_template_delete"));
    }
}
