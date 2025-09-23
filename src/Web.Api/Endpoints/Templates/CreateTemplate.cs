using Application.Dashboard;
using Application.Templates.CreateTemplateCommand;
using Infrastructure.Authorization;

namespace Web.Api.Endpoints.Templates;

internal sealed class CreateTemplateEndpoint : IEndpoint
{
    public sealed class Request
    {
        public string TemplateName { get; init; }
        public string TemplateBody { get; init; }
        public TriggerType TemplateTrigger { get; init; }
    }

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/templates", async (
            Request request,
            ICommandHandler<CreateTemplateCommand, string> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new CreateTemplateCommand(
                request.TemplateBody,
                request.TemplateName,
                request.TemplateTrigger);

            Result<string> result = await handler.Handle(command, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.Templates)
        .WithAudit("A new template was created")
        .Produces<string>(StatusCodes.Status200OK)
        .RequireAuthorization(new RequirePermissionAttribute("CreateEmailTemplates"));
    }
}
