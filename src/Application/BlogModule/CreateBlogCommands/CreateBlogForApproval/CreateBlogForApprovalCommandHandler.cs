using System;
using Domain.Blogs;

namespace Application.BlogModule.CreateBlogCommands.CreateBlogForApproval;

public sealed class CreateBlogForApprovalCommandHandler(IApplicationDbContext _dbContext) : ICommandHandler<CreateBlogForApprovalCommand, string>
{
    public async Task<Result<string>> Handle(CreateBlogForApprovalCommand command, CancellationToken cancellationToken)
    {
        var entity = new Blog
        {
            Title = command.Title,
            Content = command.Content,
            BackdropUrl = command.BackdropUrl,
        };
        _dbContext.Blog.Add(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return Result.Success("Blog submitted successfully");
    }
}



