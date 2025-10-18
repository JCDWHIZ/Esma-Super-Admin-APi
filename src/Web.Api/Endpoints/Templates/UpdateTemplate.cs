using Application.Dashboard;
using Application.Templates.UpdateTemplateCommand;
using Infrastructure.Authorization;

namespace Web.Api.Endpoints.Templates;

internal sealed class UpdateTemplateEndpoint : IEndpoint
{
    public sealed class Request
    {
        public string TemplateName { get; init; }
        public string TemplateBody { get; init; }
    }

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("/templates/{publicId}", async (
            Guid publicId,
            Request request,
            ICommandHandler<UpdateTemplateCommand, string> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new UpdateTemplateCommand(
                publicId,
                request.TemplateName,
                request.TemplateBody);

            Result<string> result = await handler.Handle(command, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.Templates)
        .WithAudit("A template was updated")
        .Produces<string>(StatusCodes.Status200OK)
        .RequireAuthorization(new RequirePermissionAttribute("email_template_edit"));
    }
}
