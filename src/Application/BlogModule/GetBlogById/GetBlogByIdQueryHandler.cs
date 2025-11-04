using Domain.Blogs;

namespace Application.BlogModule.GetBlogById;

public sealed class GetBlogByIdQueryHandler(IApplicationDbContext _context) : IQueryHandler<GetBlogByIdQuery, BlogItemDto>
{
    async Task<Result<BlogItemDto>> IQueryHandler<GetBlogByIdQuery, BlogItemDto>.Handle(GetBlogByIdQuery query, CancellationToken cancellationToken)
    {
        var blog = await _context.Blog
            .Where(x => x.PublicId == query.PublicId)
            .Select(x => new
            {
                x.PublicId,
                x.Title,
                x.Content,
                x.BackdropUrl,
                x.Status,
                x.PublishDate,
                x.RejectReason,
                x.Created,
                x.CreatedBy
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (blog is null)
        {
            return Result.Failure<BlogItemDto>(BlogErrors.NotFound(query.PublicId));
        }

        CreatorDto? creator = null;
        if (blog.CreatedBy.HasValue)
        {
            CreatorDto? user = await _context.Users
                .Where(u => u.PublicId == blog.CreatedBy.Value)
                .Select(u => new CreatorDto
                {
                    PublicId = u.PublicId,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Username = u.Username,
                    ProfilePic = u.ProfilePic
                })
                .FirstOrDefaultAsync(cancellationToken);

            creator = user;
        }
        var dto = new BlogItemDto
        {
            PublicId = blog.PublicId,
            Title = blog.Title,
            Content = blog.Content,
            BackdropUrl = blog.BackdropUrl,
            Status = blog.Status,
            PublishDate = blog.PublishDate,
            RejectReason = blog.RejectReason,
            CreatedAt = blog.Created,
            CreatedBy = creator
        };

        return Result.Success(dto);
    }
}
