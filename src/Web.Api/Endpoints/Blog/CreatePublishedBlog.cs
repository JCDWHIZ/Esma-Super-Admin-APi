// using Application.BlogModule.CreateBlogCommands.CreatePublishedBlog;
// using Application.Abstractions.Messaging;
// using SharedKernel;
// using Web.Api.Extensions;
// using Web.Api.Infrastructure;
// using Application.BlogModule;

// namespace Web.Api.Endpoints.Blog;

// internal sealed class CreatePublishedBlog : IEndpoint
// {
//     public void MapEndpoint(IEndpointRouteBuilder app)
//     {
//         app.MapPost("blogs/published", async (
//             Request request,
//             ICommandHandler<CreatePublishedBlogCommand, BlogItemDto> handler,
//             CancellationToken cancellationToken) =>
//         {
//             var command = new CreatePublishedBlogCommand
//             {
//                 Title = request.Title,
//                 Content = request.Content,
//                 BackdropUrl = request.BackdropUrl
//             };

//             Result<BlogItemDto> result = await handler.Handle(command, cancellationToken);

//             return result.Match(Results.Ok, CustomResults.Problem);
//         })
//         .WithTags(Tags.Blogs)
//         .Produces<BlogItemDto>(StatusCodes.Status200OK)
//         .RequireAuthorization();
//     }
//     public sealed record Request(string Title, string Content, string BackdropUrl);
// }
