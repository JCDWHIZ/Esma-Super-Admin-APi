using Application.Abstractions.Messaging;
using Application.BlogModule;
using Application.BlogModule.GetBlogById;
using Infrastructure.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Blog;

internal sealed class GetBlogById : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("blogs/{publicId:guid}", async (
            Guid publicId,
            IQueryHandler<GetBlogByIdQuery, BlogItemDto> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetBlogByIdQuery(publicId);

            Result<BlogItemDto> result = await handler.Handle(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.Blogs)
        .Produces<BlogItemDto>(StatusCodes.Status200OK)
        .RequireAuthorization(new RequirePermissionAttribute("blog_view"));
    }
}
