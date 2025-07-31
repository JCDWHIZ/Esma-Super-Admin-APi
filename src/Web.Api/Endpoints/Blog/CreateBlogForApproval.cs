using System;
using Application.BlogModule.CreateBlogCommands.CreateBlogForApproval;

namespace Web.Api.Endpoints.Blog;

internal sealed class CreateBlogForApproval : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("blogs/create-for-approval", async (
            CreateBlogForApprovalCommand command,
            ICommandHandler<CreateBlogForApprovalCommand, string> handler,
            CancellationToken cancellationToken) =>
        {
            Result<string> result = await handler.Handle(command, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.Blogs)
        .RequireAuthorization();
    }
}