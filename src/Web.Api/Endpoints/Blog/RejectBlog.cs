using Application.BlogModule.RejectBlogCommand;
using Infrastructure.Authorization;
namespace Web.Api.Endpoints.Blog;


internal sealed class RejectBlogEndpoint : IEndpoint
{
    public sealed class Request
    {
        public string RejectReason { get; init; }
    }

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/blogs/{publicId:guid}/reject", async (
            Guid publicId,
            Request request,
            ICommandHandler<RejectBlogCommand, string> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new RejectBlogCommand(
                publicId,
                request.RejectReason);

            Result<string> result = await handler.Handle(command, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.Blogs)
        .RequireAuthorization(new RequirePermissionAttribute("blog_reject"));
    }
}
