using Application.Dashboard;
using Application.Templates;
using Application.Templates.GetTemplateById;
using Infrastructure.Authorization;
using Web.Api.Extensions;

namespace Web.Api.Endpoints.Templates;

internal sealed class GetTemplateByIdEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/templates/{publicId}", async (
            Guid publicId,
            IQueryHandler<GetTemplateByIdQuery, TemplateDto> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetTemplateByIdQuery(publicId);

            Result<TemplateDto> result = await handler.Handle(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.Templates)
        .Produces<TemplateDto>(StatusCodes.Status200OK)
        .RequireAuthorization(new RequirePermissionAttribute("email_template_view"));
    }
}
