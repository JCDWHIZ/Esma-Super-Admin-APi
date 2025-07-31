using System;

namespace Application.BlogModule.CreateBlogCommands.CreateBlogForApproval;

public sealed record CreateBlogForApprovalCommand : ICommand<string>
{
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string BackdropUrl { get; set; } = string.Empty;
}