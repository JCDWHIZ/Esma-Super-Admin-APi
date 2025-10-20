namespace Application.BlogModule.GetBlogsWithPagination;

public sealed record GetBlogWithPaginationQuery : IQuery<PaginatedList<BlogItemDto>>
{
    public string? Title { get; set; }
    public string? Content { get; set; }
    public string? BackdropUrl { get; set; }
    public BlogStatus? Status { get; set; }
    public DateTime? PublishDate { get; set; }
    public Guid? UserPublicId { get; set; }
    public int? PageNumber { get; set; } = 1;
    public int? PageSize { get; set; } = 10;
}
