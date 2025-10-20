using Application;
using Application.BlogModule;
using Application.BlogModule.GetBlogsWithPagination;
using Infrastructure.Authorization;

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
            Guid? AdminPublicId,
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
                UserPublicId = AdminPublicId,
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            Result<PaginatedList<BlogItemDto>> result = await handler.Handle(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.Blogs)
        .Produces<PaginatedList<BlogItemDto>>()
        .RequireAuthorization(new RequirePermissionAttribute("blog_view"));
    }
}
