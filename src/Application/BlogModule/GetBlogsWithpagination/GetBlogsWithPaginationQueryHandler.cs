namespace Application.BlogModule.GetBlogsWithPagination;

public sealed class GetBlogsWithPaginationQueryHandler(IApplicationDbContext context)
    : IQueryHandler<GetBlogWithPaginationQuery, PaginatedList<BlogItemDto>>
{
    public async Task<Result<PaginatedList<BlogItemDto>>> Handle(GetBlogWithPaginationQuery query, CancellationToken cancellationToken)
    {
        IQueryable<BlogItemDto> blogQuery =
            from blog in context.Blog
            join user in context.Users on blog.CreatedBy equals user.PublicId into userJoin
            from user in userJoin.DefaultIfEmpty()
            select new BlogItemDto
            {
                PublicId = blog.PublicId,
                Title = blog.Title,
                Content = blog.Content,
                BackdropUrl = blog.BackdropUrl,
                Status = blog.Status,
                PublishDate = blog.PublishDate,
                CreatedAt = blog.Created,
                RejectReason = blog.RejectReason,
                CreatedBy = user == null
                    ? null
                    : new CreatorDto
                    {
                        PublicId = user.PublicId,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Username = user.Username,
                        ProfilePic = user.ProfilePic
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
            query.PageSize ?? 10);

        return Result.Success(blogList);
    }
}
