namespace Application.BlogModule;

public sealed record BlogItemDto
{
    public Guid PublicId { get; set; }
    public string BackdropUrl { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public BlogStatus Status { get; set; } = BlogStatus.DRAFT;
    public DateTime? PublishDate { get; set; }
    public DateTimeOffset? CreatedAt { get; set; }
    public Guid? CreatedBy { get; set; }
}
