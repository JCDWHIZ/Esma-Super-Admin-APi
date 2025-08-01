using System;
using Domain.Blogs;
using Hangfire;
using Microsoft.EntityFrameworkCore;

namespace Application.BlogModule.CreateBlogCommands.CreateScheduledBlog;


public sealed class CreateScheduledBlogCommandHandler(IApplicationDbContext _dbContext) : ICommandHandler<CreateScheduledBlogCommand, BlogItemDto>
{
    async Task<Result<BlogItemDto>> ICommandHandler<CreateScheduledBlogCommand, BlogItemDto>.Handle(CreateScheduledBlogCommand command, CancellationToken cancellationToken)
    {
        var entity = new Blog
        {
            Title = command.Title,
            Content = command.Content,
            BackdropUrl = command.BackdropUrl,
            Status = BlogStatus.SCHEDULED,
            PublishDate = command.PublishDate,
        };
        if (await _dbContext.Blog.AnyAsync(x => x.Title == command.Title, cancellationToken))
        {
            return Result.Failure<BlogItemDto>(BlogErrors.BlogAlreadyExists(command.Title));
        }
        _dbContext.Blog.Add(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);
        if (command.PublishDate.HasValue && command.PublishDate > DateTime.UtcNow)
        {
            BackgroundJob.Schedule(() => PublishBlog(entity.PublicId, cancellationToken), command.PublishDate.Value - DateTime.UtcNow);
        }
        return Result.Success(new BlogItemDto
        {
            PublicId = entity.PublicId,
            Title = entity.Title,
            Content = entity.Content,
            BackdropUrl = entity.BackdropUrl,
            Status = entity.Status,
            PublishDate = entity.PublishDate,
            CreatedAt = entity.Created
        });
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