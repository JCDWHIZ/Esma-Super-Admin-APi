using Application.BlogModule.DeleteBlogCommand;
using Application.Abstractions.Messaging;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Blog;

internal sealed class DeleteBlog : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("blogs/{publicId:guid}", async (
            Guid publicId,
            ICommandHandler<DeleteBlogCommand, string> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new DeleteBlogCommand(publicId);

            Result<string> result = await handler.Handle(command, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.Blogs);
        // .RequireAuthorization();
    }
}
