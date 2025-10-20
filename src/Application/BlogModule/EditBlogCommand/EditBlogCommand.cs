namespace Application.BlogModule.EditBlogCommand;

public sealed record EditBlogCommand : ICommand<string>
{
    public Guid PublicId { get; init; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string BackdropUrl { get; set; } = string.Empty;
    public BlogStatus Status { get; set; }
    public DateTime? PublishDate { get; set; }
}
