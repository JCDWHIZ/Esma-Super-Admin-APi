using Application;
using Application.Dashboard;
using Application.Templates;
using Application.Templates.GetTemplates;
using Infrastructure.Authorization;
using Web.Api.Extensions;

namespace Web.Api.Endpoints.Templates;

internal sealed class GetTemplatesWithPaginationEndpoint : IEndpoint
{
    public sealed class Request
    {
        public int? Page { get; init; }
        public int? PageSize { get; init; }
    }

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/templates", async (
            [AsParameters] Request request,
            IQueryHandler<GetTemplatesWithPaginationQuery, PaginatedList<TemplateDto>> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetTemplatesWithPaginationQuery(
                request.Page,
                request.PageSize);

            Result<PaginatedList<TemplateDto>> result = await handler.Handle(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.Templates)
        .Produces<PaginatedList<TemplateDto>>(StatusCodes.Status200OK)
        .RequireAuthorization(new RequirePermissionAttribute("ViewEmailTemplates"));
    }
}
