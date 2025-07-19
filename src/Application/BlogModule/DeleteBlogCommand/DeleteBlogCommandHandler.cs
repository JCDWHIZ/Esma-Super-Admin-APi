using System;
using Domain.Blogs;
using Application.Interfaces;

namespace Application.BlogModule.DeleteBlogCommand;

public sealed class DeleteBlogCommandHandler(IApplicationDbContext _dbContext) : ICommandHandler<DeleteBlogCommand, string>
{
    async Task<Result<string>> ICommandHandler<DeleteBlogCommand, string>.Handle(DeleteBlogCommand command, CancellationToken cancellationToken)
    {
        Blog? blog = await _dbContext.Blog.FirstOrDefaultAsync(x => x.PublicId == command.PublicId, cancellationToken);

        if (blog == null)
        {
            return Result.Failure<string>(BlogErrors.NotFound(command.PublicId));
        }
        _dbContext.Blog.Remove(blog);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return Result.Success("Blog deleted successfully");
    }
}
