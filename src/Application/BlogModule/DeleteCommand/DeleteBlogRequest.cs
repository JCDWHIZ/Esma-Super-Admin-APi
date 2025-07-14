using System;
using admin_service.Application.Common.Interfaces;

namespace admin_service.Application.BlogModule.Commands.DeleteCommand;

public record DeleteBlogRequestCommand(string PublicId) : IRequest;

public class DeleteBlogRequestHandler : IRequestHandler<DeleteBlogRequestCommand>
{
    private readonly IApplicationDbContext _dbContext;

    public DeleteBlogRequestHandler(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Handle(DeleteBlogRequestCommand request, CancellationToken cancellationToken)
    {
        var blog = _dbContext.Blog.FirstOrDefault(x => x.PublicId == request.PublicId);
        Guard.Against.NotFound(request.PublicId, blog);
        if (blog != null){
            _dbContext.Blog.Remove(blog);
        }
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
