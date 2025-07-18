using System;
using admin_service.Application.Common.Exceptions;
using Application.Interfaces;
using admin_service.Domain.Enums;

namespace admin_service.Application.BlogModule.Commands.CreateCommands.PublishBlog;

public class InitiateBlogPublish : ICommand
{
    public string PublicId { get; set; } = string.Empty;
}

public class InitiateBlogPublishHandler : ICommandHandler<InitiateBlogPublish>
{
    private readonly IApplicationDbContext _dbContext;

    public InitiateBlogPublishHandler(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Handle(InitiateBlogPublish request, CancellationToken cancellationToken)
    {
        var blog = _dbContext.Blog.FirstOrDefault(x => x.PublicId == request.PublicId);
        Guard.Against.NotFound(request.PublicId, blog);
        if (blog != null)
        {
            blog.Status = BlogStatus.PUBLISHED;
            blog.PublishDate = DateTime.UtcNow;
        }
        await _dbContext.SaveChangesAsync(default);

        // return blog.PublicId;
    }

}