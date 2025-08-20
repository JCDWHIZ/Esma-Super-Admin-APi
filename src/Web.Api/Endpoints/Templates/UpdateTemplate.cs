using Application.Templates.UpdateTemplateCommand;

namespace Web.Api.Endpoints.Templates;

internal sealed class UpdateTemplateEndpoint : IEndpoint
{
    public sealed class Request
    {
        public Guid PublicId { get; init; }
        public string TemplateName { get; init; }
        public string TemplateBody { get; init; }
        public TriggerType TemplateTrigger { get; init; }
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
                request.TemplateBody,
                request.TemplateTrigger);

            Result<string> result = await handler.Handle(command, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.Templates)
        .RequireAuthorization();
    }
}
