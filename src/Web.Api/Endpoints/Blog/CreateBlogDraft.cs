using System;
using Application.BlogModule;
using Application.BlogModule.CreateBlogCommands.CreateBlogDraft;

namespace Web.Api.Endpoints.Blog;

internal sealed class CreateBlogDraft : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("blogs/drafts", async (
            Request request,
            ICommandHandler<CreateBlogDraftCommand, BlogItemDto> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new CreateBlogDraftCommand
            {
                Title = request.Title,
                Content = request.Content,
                BackdropUrl = request.BackdropUrl
            };

            Result<BlogItemDto> result = await handler.Handle(command, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.Blogs)
        .RequireAuthorization();
    }

    public sealed record Request(string Title, string Content, string BackdropUrl);
}
