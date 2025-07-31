using System;
using Domain.Blogs;
using Hangfire;

namespace Application.BlogModule.ScheduleBlog;

public sealed class ScheduleBlogCommandHandler(IApplicationDbContext _dbContext) : ICommandHandler<ScheduleBlogCommand, string>
{
    public async Task<Result<string>> Handle(ScheduleBlogCommand command, CancellationToken cancellationToken)
    {
        Blog? existingBlog = await _dbContext.Blog.FirstOrDefaultAsync(x => x.PublicId == command.PublicId, cancellationToken);

        if (existingBlog == null)
        {
            return Result.Failure<string>(BlogErrors.NotFound(command.PublicId));
        }

        if (command.PublishDate.HasValue && command.PublishDate > DateTime.UtcNow)
        {
            BackgroundJob.Schedule(() => PublishBlog(existingBlog.PublicId, cancellationToken), command.PublishDate.Value - DateTime.UtcNow);
        }

        return Result.Success("Blog Scheduled");
    }

    public async Task PublishBlog(Guid blogId, CancellationToken cancellationToken)
    {
        Blog? blog = await _dbContext.Blog.FirstOrDefaultAsync(x => x.PublicId == blogId, cancellationToken);
        if (blog == null)
        {
            Result.Failure<BlogItemDto>(BlogErrors.NotFound(blogId));
        }
        if (blog != null)
        {
            blog.Status = BlogStatus.PUBLISHED;
            blog.PublishDate = DateTime.UtcNow;
        }

        await _dbContext.SaveChangesAsync(default);
    }
}
