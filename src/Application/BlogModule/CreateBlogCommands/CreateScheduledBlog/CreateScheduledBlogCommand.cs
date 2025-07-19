namespace Application.BlogModule.CreateBlogCommands.CreateScheduledBlog;

public record CreateScheduledBlogCommand : ICommand<BlogItemDto>
{
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string BackdropUrl { get; set; } = string.Empty;
    public BlogStatus Status { get; set; } = BlogStatus.DRAFT;
    public DateTime? PublishDate { get; set; }
}
