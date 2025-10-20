using Application.BlogModule.ScheduleBlog;
using Infrastructure.Authorization;

namespace Web.Api.Endpoints.Blog;

public class ScheduleBlog : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("blogs/schedule", async (
            ScheduleBlogCommand command,
            ICommandHandler<ScheduleBlogCommand, string> handler,
            CancellationToken cancellationToken) =>
        {
            Result<string> result = await handler.Handle(command, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.Blogs)
        .WithAudit("Scheduled Blog")
        .RequireAuthorization(new RequirePermissionAttribute("blog_schedule"));
    }
}
