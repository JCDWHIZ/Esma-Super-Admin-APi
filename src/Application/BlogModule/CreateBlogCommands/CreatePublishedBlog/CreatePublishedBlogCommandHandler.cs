using Domain.Blogs;

namespace Application.BlogModule.CreateBlogCommands.CreatePublishedBlog;

public sealed class InitiatePublishedBlogRequest(IApplicationDbContext _dbContext) : ICommandHandler<CreatePublishedBlogCommand, BlogItemDto>
{

    async Task<Result<BlogItemDto>> ICommandHandler<CreatePublishedBlogCommand, BlogItemDto>.Handle(CreatePublishedBlogCommand command, CancellationToken cancellationToken)
    {
        var entity = new Blog
        {
            Title = command.Title,
            Content = command.Content,
            BackdropUrl = command.BackdropUrl,
            Status = BlogStatus.PUBLISHED,
        };
        _dbContext.Blog.Add(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return Result.Success(new BlogItemDto
        {
            Id = entity.Id,
            Title = entity.Title,
            Content = entity.Content,
            BackdropUrl = entity.BackdropUrl,
            Status = entity.Status
        });
    }
}
