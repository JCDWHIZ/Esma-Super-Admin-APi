using Application.Abstractions.Messaging;
using Application.BlogModule.CreateBlogCommands.PublishBlog;
using Infrastructure.Authorization;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Blog;

internal sealed class PublishBlog : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("blogs/{publicId:guid}/publish", async (
            Guid publicId,
            ICommandHandler<PublishBlogCommand, string> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new PublishBlogCommand(publicId);

            Result<string> result = await handler.Handle(command, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.Blogs)
        .Produces<string>(StatusCodes.Status200OK)
        .WithAudit("Published Blog")
        .RequireAuthorization(new RequirePermissionAttribute("blog_publish"));
    }
}
