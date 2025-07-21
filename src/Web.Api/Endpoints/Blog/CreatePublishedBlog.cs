using Application.BlogModule.CreateBlogCommands.CreatePublishedBlog;
using Application.Abstractions.Messaging;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;
using Application.BlogModule;

namespace Web.Api.Endpoints.Blog;

internal sealed class CreatePublishedBlog : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("blogs/published", async (
            string title,
            string content,
            string backdropUrl,
            BlogStatus status,
            ICommandHandler<CreatePublishedBlogCommand, BlogItemDto> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new CreatePublishedBlogCommand
            {
                Title = title,
                Content = content,
                BackdropUrl = backdropUrl,
                Status = status
            };

            Result<BlogItemDto> result = await handler.Handle(command, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.Blogs);
        // .RequireAuthorization();
    }
}
