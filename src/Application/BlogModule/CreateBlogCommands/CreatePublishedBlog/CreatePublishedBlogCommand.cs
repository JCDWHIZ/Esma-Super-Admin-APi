using System;
using Application.Interfaces;

namespace Application.BlogModule.CreateBlogCommands.CreatePublishedBlog;


public sealed record CreatePublishedBlogCommand : ICommand<BlogItemDto>
{
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string BackdropUrl { get; set; } = string.Empty;
    public BlogStatus Status { get; set; } = BlogStatus.DRAFT;
}