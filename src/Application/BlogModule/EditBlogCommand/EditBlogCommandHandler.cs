using System;
using Domain.Blogs;
using Hangfire;

namespace Application.BlogModule.EditBlogCommand;


public sealed class EditBlogCommandHandler(IApplicationDbContext _dbContext) : ICommandHandler<EditBlogCommand, string>
{
    async Task<Result<string>> ICommandHandler<EditBlogCommand, string>.Handle(EditBlogCommand command, CancellationToken cancellationToken)
    {
        Blog? blog = await _dbContext.Blog.FirstOrDefaultAsync(x => x.PublicId == command.PublicId, cancellationToken);

        if (blog is null)
        {
            return Result.Failure<string>(BlogErrors.NotFound(command.PublicId));
        }

        blog.Title = command.Title;
        blog.Content = command.Content;
        blog.BackdropUrl = command.BackdropUrl;
        blog.Status = command.Status;
        blog.PublishDate = command.PublishDate;
        blog.RejectReason = string.Empty;

        await _dbContext.SaveChangesAsync(cancellationToken);

        if (command.Status == BlogStatus.SCHEDULED && command.PublishDate.HasValue && command.PublishDate > DateTime.UtcNow)
        {
            blog.Status = BlogStatus.SCHEDULED;

            BackgroundJob.Schedule(
                () => PublishBlog(blog.PublicId, CancellationToken.None),
                command.PublishDate.Value - DateTime.UtcNow
            );
        }
        else if (command.Status == BlogStatus.DRAFT)
        {
            blog.Status = BlogStatus.DRAFT;
        }
        else if (command.Status == BlogStatus.PUBLISHED)
        {
            // Optional: Only allow publishing immediately if explicitly triggered
            blog.Status = BlogStatus.PUBLISHED;
            blog.PublishDate = DateTime.UtcNow;
        }
        return Result.Success("Blog updated successfully");
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


