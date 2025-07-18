using System;
using admin_service.Application.Common.Exceptions;
using Application.Interfaces;
using admin_service.Domain.Entities;
using admin_service.Domain.Enums;
using Hangfire;

namespace admin_service.Application.BlogModule.Commands.CreateScheduledBlog;

public record InitiateScheduledBlogRequestCommand : ICommand<BlogItemDto>
{
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string BackdropUrl { get; set; } = string.Empty;
    public BlogStatus Status { get; set; } = BlogStatus.DRAFT;
    public DateTime? PublishDate { get; set; }
}

public class InitiateScheduledBlogRequest
    : ICommandHandler<InitiateScheduledBlogRequestCommand, BlogItemDto>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IMapper _mapper;

    public InitiateScheduledBlogRequest(IApplicationDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<BlogItemDto> Handle(
        InitiateScheduledBlogRequestCommand request,
        CancellationToken cancellationToken
    )
    {
        var entity = new Blog
        {
            Title = request.Title,
            Content = request.Content,
            BackdropUrl = request.BackdropUrl,
            Status = BlogStatus.SCHEDULED,
            PublishDate = request.PublishDate,
        };
        if (_dbContext.Blog.Any(x => x.Title == request.Title))
        {
            throw new AlreadyExistsException("Title already exists");
        }
        _dbContext.Blog.Add(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);
        if (request.PublishDate.HasValue && request.PublishDate > DateTime.UtcNow)
        {
            BackgroundJob.Schedule(
                () => PublishBlog(entity.Id),
                request.PublishDate.Value - DateTime.UtcNow
            );
        }
        return _mapper.Map<BlogItemDto>(entity);
    }

    public async Task PublishBlog(int blogId)
    {
        var blog = _dbContext.Blog.FirstOrDefault(x => x.Id == blogId);
        Guard.Against.NotFound(blogId, blog);
        if (blog != null)
        {
            blog.Status = BlogStatus.PUBLISHED;
            blog.PublishDate = DateTime.UtcNow;
        }

        await _dbContext.SaveChangesAsync(default);
    }
}
