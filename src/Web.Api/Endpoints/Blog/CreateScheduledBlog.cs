using Application.BlogModule.CreateBlogCommands.CreateScheduledBlog;
using Application.Abstractions.Messaging;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;
using Application.BlogModule;

namespace Web.Api.Endpoints.Blog;

internal sealed class CreateScheduledBlog : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("blogs/scheduled", async (
            string title,
            string content,
            string backdropUrl,
            BlogStatus status,
            DateTime? publishDate,
            ICommandHandler<CreateScheduledBlogCommand, BlogItemDto> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new CreateScheduledBlogCommand
            {
                Title = title,
                Content = content,
                BackdropUrl = backdropUrl,
                Status = status,
                PublishDate = publishDate
            };

            Result<BlogItemDto> result = await handler.Handle(command, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.Blogs);
        // .RequireAuthorization();
    }
}
