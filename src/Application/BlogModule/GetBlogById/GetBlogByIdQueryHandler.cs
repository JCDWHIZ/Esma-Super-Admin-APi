using System;
using Application.Interfaces;
using Domain.Blogs;

namespace Application.BlogModule.GetBlogById;

public sealed class GetBlogByIdQueryHandler(IApplicationDbContext _context) : IQueryHandler<GetBlogByIdQuery, BlogItemDto>
{
    async Task<Result<BlogItemDto>> IQueryHandler<GetBlogByIdQuery, BlogItemDto>.Handle(GetBlogByIdQuery query, CancellationToken cancellationToken)
    {
        Blog? entity = await _context.Blog
            .FirstOrDefaultAsync(x => x.PublicId == query.PublicId, cancellationToken);
        if (entity is null)
        {
            return Result.Failure<BlogItemDto>(BlogErrors.NotFound(query.PublicId));
        }
        return Result.Success(new BlogItemDto
        {
            PublicId = entity.PublicId,
            Title = entity.Title,
            Content = entity.Content,
            BackdropUrl = entity.BackdropUrl,
            Status = entity.Status,
            PublishDate = entity.PublishDate,
            RejectReason = entity.RejectReason,
            CreatedAt = entity.Created,
            CreatedBy = entity.CreatedBy
        });
    }
}
