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

        if (await _dbContext.Blog.AnyAsync(x => x.Title == command.Title && x.PublicId != command.PublicId, cancellationToken))
        {
            return Result.Failure<string>(BlogErrors.BlogAlreadyExists(command.Title));
        }

        blog.Title = command.Title;
        blog.Content = command.Content;
        blog.BackdropUrl = command.BackdropUrl;
        blog.Status = command.Status;
        blog.PublishDate = command.PublishDate;

        await _dbContext.SaveChangesAsync(cancellationToken);

        if (command.Status == BlogStatus.SCHEDULED && command.PublishDate.HasValue && command.PublishDate > DateTime.UtcNow)
        {
            BackgroundJob.Schedule(() => PublishBlog(blog.PublicId, cancellationToken), command.PublishDate.Value - DateTime.UtcNow);
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


