using System;
using Application.Interfaces;
using Domain.Blogs;

namespace Application.BlogModule.CreateBlogCommands.PublishBlog;

public sealed class PublishBlogCommandHandler(IApplicationDbContext _dbContext) : ICommandHandler<PublishBlogCommand, string>
{
    async Task<Result<string>> ICommandHandler<PublishBlogCommand, string>.Handle(PublishBlogCommand command, CancellationToken cancellationToken)
    {
        Blog? blog = await _dbContext.Blog.FirstOrDefaultAsync(x => x.PublicId == command.PublicId, cancellationToken);
        if (blog == null)
        {
            return Result.Failure<string>(BlogErrors.NotFound(command.PublicId));
        }
        blog.Status = BlogStatus.PUBLISHED;
        blog.PublishDate = DateTime.UtcNow;
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success("Blog published successfully");
    }
}