using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Blogs;
using Hangfire;
using Microsoft.EntityFrameworkCore;

namespace Application.BlogModule.RejectBlogCommand;
public sealed class RejectBlogCommandHandler(IApplicationDbContext _context) : ICommandHandler<RejectBlogCommand, string>
{
    async Task<Result<string>> ICommandHandler<RejectBlogCommand, string>.Handle(RejectBlogCommand command, CancellationToken cancellationToken)
    {
        Blog? blog = await _context.Blog.FirstOrDefaultAsync(x => x.PublicId == command.PublicId, cancellationToken);

        if (blog is null)
        {
            return Result.Failure<string>(BlogErrors.NotFound(command.PublicId));
        }

        blog.Status = BlogStatus.REJECTED;
        blog.RejectReason = command.RejectReason;

        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success("Blog rejected successfully");
    }
}
