using System;
using System.Threading;
using System.Threading.Tasks;
using admin_service.Application.Common.Exceptions;
using admin_service.Application.Common.Interfaces;
using admin_service.Domain.Entities;
using admin_service.Domain.Enums;
using AutoMapper;
using Hangfire;
using MediatR;

namespace admin_service.Application.BlogModule.Commands.EditBlog;

public record EditBlogRequestCommand : IRequest<BlogItemDto>
{
    public string PublicId { get; init; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string BackdropUrl { get; set; } = string.Empty;
    public BlogStatus Status { get; set; }
    public DateTime? PublishDate { get; set; }
}

public class EditBlogRequestHandler : IRequestHandler<EditBlogRequestCommand, BlogItemDto>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IMapper _mapper;

    public EditBlogRequestHandler(IApplicationDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<BlogItemDto> Handle(EditBlogRequestCommand request, CancellationToken cancellationToken)
    {
        var blog = _dbContext.Blog.FirstOrDefault(x => x.PublicId == request.PublicId);
        Guard.Against.NotFound(request.PublicId, blog);
        if (_dbContext.Blog.Any(x => x.Title == request.Title && x.PublicId != request.PublicId))
        {
            throw new AlreadyExistsException("Title already exists");
        }

        blog.Title = request.Title;
        blog.Content = request.Content;
        blog.BackdropUrl = request.BackdropUrl;
        blog.Status = request.Status;
        blog.PublishDate = request.PublishDate;

        await _dbContext.SaveChangesAsync(cancellationToken);

        if (request.Status == BlogStatus.SCHEDULED && request.PublishDate.HasValue && request.PublishDate > DateTime.UtcNow)
        {
            BackgroundJob.Schedule(() => PublishBlog(blog.Id), request.PublishDate.Value - DateTime.UtcNow);
        }

        return _mapper.Map<BlogItemDto>(blog);
    }
    public async Task PublishBlog(int blogId)
    {
        var blog = _dbContext.Blog.FirstOrDefault(x => x.Id == blogId);
        Guard.Against.NotFound(blogId, blog);
        if (blog != null){
        blog.Status = BlogStatus.PUBLISHED;
        blog.PublishDate = DateTime.UtcNow;
        }
        
        await _dbContext.SaveChangesAsync(default);
    }
}
