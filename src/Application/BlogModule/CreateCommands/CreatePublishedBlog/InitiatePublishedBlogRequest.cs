using System;
using admin_service.Application.Common.Exceptions;
using admin_service.Application.Common.Interfaces;
using admin_service.Domain.Entities;
using admin_service.Domain.Enums;

namespace admin_service.Application.BlogModule.Commands.CreatePublishedBlog;


public record IntiatePublishedBlogRequestCommand : ICommand<BlogItemDto>
{
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string BackdropUrl { get; set; } = string.Empty;
    public BlogStatus Status { get; set; } = BlogStatus.DRAFT;
}

public class InitiatePublishedBlogRequest : ICommandHandler<IntiatePublishedBlogRequestCommand, BlogItemDto>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IMapper _mapper;

    public InitiatePublishedBlogRequest
    (IApplicationDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<BlogItemDto> Handle(IntiatePublishedBlogRequestCommand request, CancellationToken cancellationToken)
    {
        var entity = new Blog
        {
            Title = request.Title,
            Content = request.Content,
            BackdropUrl = request.BackdropUrl,
            Status = BlogStatus.PUBLISHED,
        };
        if (_dbContext.Blog.Any(x => x.Title == request.Title))
        {
            throw new AlreadyExistsException("Title already exists");
        }
        _dbContext.Blog.Add(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return _mapper.Map<BlogItemDto>(entity);
    }
}
