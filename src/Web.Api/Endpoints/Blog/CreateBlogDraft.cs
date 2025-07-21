using System;
using Application.BlogModule;
using Application.BlogModule.CreateBlogCommands.CreateBlogDraft;

namespace Web.Api.Endpoints.Blog;

internal sealed class CreateBlogDraft : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("blogs/drafts", async (
            string title,
            string content,
            string backdropUrl,
            BlogStatus status,
            ICommandHandler<CreateBlogDraftCommand, BlogItemDto> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new CreateBlogDraftCommand
            {
                Title = title,
                Content = content,
                BackdropUrl = backdropUrl,
                Status = status
            };

            Result<BlogItemDto> result = await handler.Handle(command, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.Blogs)
        .RequireAuthorization();
    }

}
