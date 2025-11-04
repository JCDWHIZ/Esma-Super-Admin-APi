namespace Application.BlogModule;

public sealed record BlogItemDto
{
    public Guid PublicId { get; set; }
    public string BackdropUrl { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string? RejectReason { get; set; } = string.Empty;
    public BlogStatus Status { get; set; } = BlogStatus.DRAFT;
    public DateTime? PublishDate { get; set; }
    public DateTimeOffset? CreatedAt { get; set; }
    public CreatorDto? CreatedBy { get; set; }
}


public sealed record CreatorDto
{
    public Guid PublicId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string? ProfilePic { get; set; }
}
