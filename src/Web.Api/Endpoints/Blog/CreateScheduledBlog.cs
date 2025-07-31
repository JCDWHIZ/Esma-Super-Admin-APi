// using Application.BlogModule.CreateBlogCommands.CreateScheduledBlog;
// using Application.Abstractions.Messaging;
// using SharedKernel;
// using Web.Api.Extensions;
// using Web.Api.Infrastructure;
// using Application.BlogModule;

// namespace Web.Api.Endpoints.Blog;

// internal sealed class CreateScheduledBlog : IEndpoint
// {
//     public void MapEndpoint(IEndpointRouteBuilder app)
//     {
//         app.MapPost("blogs/scheduled", async (
//             Request request,
//             ICommandHandler<CreateScheduledBlogCommand, BlogItemDto> handler,
//             CancellationToken cancellationToken) =>
//         {
//             var command = new CreateScheduledBlogCommand
//             {
//                 Title = request.Title,
//                 Content = request.Content,
//                 BackdropUrl = request.BackdropUrl,
//                 PublishDate = request.PublishDate
//             };

//             Result<BlogItemDto> result = await handler.Handle(command, cancellationToken);

//             return result.Match(Results.Ok, CustomResults.Problem);
//         })
//         .WithTags(Tags.Blogs)
//         .Produces<BlogItemDto>(StatusCodes.Status200OK)
//         .RequireAuthorization();
//     }
//     public sealed record Request(string Title, string Content, string BackdropUrl, DateTime PublishDate);
// }
