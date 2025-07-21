using Application.BlogModule.EditBlogCommand;
using Application.Abstractions.Messaging;
using Microsoft.AspNetCore.Mvc;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Blog;

internal sealed class UpdateBlog : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("blogs/{publicId:guid}", async (
            Guid publicId,
            EditBlogCommand body,
            ICommandHandler<EditBlogCommand, string> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new EditBlogCommand
            {
                PublicId = publicId,
                Title = body.Title,
                Content = body.Content,
                BackdropUrl = body.BackdropUrl,
                Status = body.Status,
                PublishDate = body.PublishDate
            };

            Result<string> result = await handler.Handle(command, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.Blogs)
        .RequireAuthorization();
    }
}
