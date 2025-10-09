using System;
using Application.BlogModule;
using Application.BlogModule.CreateBlogCommands.CreateBlogDraft;
using Infrastructure.Authorization;

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
        .Produces<BlogItemDto>(StatusCodes.Status200OK)
        .WithAudit("Create Blog Drafts")
        .RequireAuthorization(new RequirePermissionAttribute("blog_create"));
    }

    public sealed record Request(string Title, string Content, string BackdropUrl);
}
