namespace Application.BlogModule.GetBlogsWithPagination;

public sealed class GetBlogsWithPaginationQueryHandler(IApplicationDbContext _context) : IQueryHandler<GetBlogWithPaginationQuery, PaginatedList<BlogItemDto>>
{
    async Task<Result<PaginatedList<BlogItemDto>>> IQueryHandler<GetBlogWithPaginationQuery, PaginatedList<BlogItemDto>>.Handle(GetBlogWithPaginationQuery query, CancellationToken cancellationToken)
    {
        IQueryable<BlogItemDto> blogQuery = from b in _context.Blog
                        join u in _context.Users on b.CreatedBy equals u.PublicId into userJoin
                        from u in userJoin.DefaultIfEmpty()
                        select new BlogItemDto
                        {
                            PublicId = b.PublicId,
                            Title = b.Title,
                            Content = b.Content,
                            BackdropUrl = b.BackdropUrl,
                            Status = b.Status,
                            PublishDate = b.PublishDate,
                            CreatedAt = b.Created,
                            RejectReason = b.RejectReason,
                            CreatedBy = u == null ? null : new CreatorDto
                            {
                                PublicId = u.PublicId,
                                FirstName = u.FirstName,
                                LastName = u.LastName,
                                Username = u.Username,
                                ProfilePic = u.ProfilePic
                            }
                        };

        if (!string.IsNullOrEmpty(query.Title))
        {
            blogQuery = blogQuery.Where(b => b.Title.Contains(query.Title));
        }

        if (!string.IsNullOrEmpty(query.Content))
        {
            blogQuery = blogQuery.Where(b => b.Content.Contains(query.Content));
        }

        if (!string.IsNullOrEmpty(query.BackdropUrl))
        {
            blogQuery = blogQuery.Where(b => b.BackdropUrl.Contains(query.BackdropUrl));
        }

        if (query.Status.HasValue)
        {
            blogQuery = blogQuery.Where(b => b.Status == query.Status.Value);
        }

        if (query.PublishDate.HasValue)
        {
            blogQuery = blogQuery.Where(b => b.PublishDate == query.PublishDate.Value);
        }

        if (query.UserPublicId.HasValue)
        {
            blogQuery = blogQuery.Where(b => b.CreatedBy != null && b.CreatedBy.PublicId == query.UserPublicId.Value);
        }

        PaginatedList<BlogItemDto> blogList = await PaginatedList<BlogItemDto>.CreateAsync(
            blogQuery,
            query.PageNumber ?? 1,
            query.PageSize ?? 10
        );

        return Result.Success(blogList);
    }
}
