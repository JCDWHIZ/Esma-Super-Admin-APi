using Application.BlogModule.GetBlogsWithPagination;
using Application.Abstractions.Messaging;
using Microsoft.AspNetCore.Mvc;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;
using System.Globalization;
using Application;
using Application.BlogModule;

namespace Web.Api.Endpoints.Blog;

internal sealed class GetBlogs : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("blogs", async (
            string? title,
            string? content,
            string? backdropUrl,
            BlogStatus? status,
            DateTime? publishDate,
            int? pageNumber,
            int? pageSize,
            IQueryHandler<GetBlogWithPaginationQuery, PaginatedList<BlogItemDto>> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetBlogWithPaginationQuery
            {
                Title = title,
                Content = content,
                BackdropUrl = backdropUrl,
                Status = status,
                PublishDate = publishDate,
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            Result<PaginatedList<BlogItemDto>> result = await handler.Handle(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.Blogs)
        .RequireAuthorization();
    }
}
