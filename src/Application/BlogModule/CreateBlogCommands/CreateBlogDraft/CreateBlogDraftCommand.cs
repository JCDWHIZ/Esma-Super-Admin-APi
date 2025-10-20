namespace Application.BlogModule.CreateBlogCommands.CreateBlogDraft;

public sealed record CreateBlogDraftCommand : ICommand<BlogItemDto>
{
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string BackdropUrl { get; set; } = string.Empty;
    public BlogStatus Status { get; set; } = BlogStatus.DRAFT;
}
