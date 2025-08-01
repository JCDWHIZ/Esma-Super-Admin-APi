using System;
using Domain.Blogs;

namespace Application.BlogModule.CreateBlogCommands.CreateBlogDraft;

public sealed class InitiateBlogDraftReqeuest(IApplicationDbContext _dbContext) : ICommandHandler<CreateBlogDraftCommand, BlogItemDto>
{

    async Task<Result<BlogItemDto>> ICommandHandler<CreateBlogDraftCommand, BlogItemDto>.Handle(CreateBlogDraftCommand command, CancellationToken cancellationToken)
    {
        var entity = new Blog
        {
            Title = command.Title,
            Content = command.Content,
            BackdropUrl = command.BackdropUrl,
            Status = BlogStatus.DRAFT,
        };
        _dbContext.Blog.Add(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);
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
}